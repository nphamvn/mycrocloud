using System.Net;
using MockServer.Core.Enums;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.ReverseProxyServer.Constraints;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Middlewares;
public class RequestValidation : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRouteResolver _routeService;
    private readonly IProjectRepository _projectRepository;
    private readonly ICacheService _cacheService;
    private readonly ModelBinderProvider _modelBinderProvider;
    public RequestValidation(
            IRequestRepository requestRepository,
            ICacheService cacheService,
            IRouteResolver routeService,
            IProjectRepository projectRepository,
            ModelBinderProvider modelBinderProvider)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _cacheService = cacheService;
        _routeService = routeService;
        _modelBinderProvider = modelBinderProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var req = GrabRequest(context.Request);

        var p = await _projectRepository.Get(req.Username, req.ProjectName);
        if (p == default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{req.ProjectName}' project is not found");
            return;
        }

        if (p.Authorization == 1)
        {
            var options = await _projectRepository.GetJwtHandlerConfiguration(p.Id);
            var tokenSource = options.TokenBinderSource;

            var binder = _modelBinderProvider.GetBinder(tokenSource);
            var token = binder.Get(context) as string;
            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("No token found");
                return;
            }
            options.Claims = new Dictionary<string, string>();
            foreach (var config in options.ClaimsBinderSource)
            {
                var claimType = config.Key;
                var binderSource = config.Value;
                binder = _modelBinderProvider.GetBinder(binderSource);
                var value = binder.Get(context);
                if (value != null)
                {
                    options.Claims[claimType] = value as string;
                }
            }
            //TODO: Create instance with FactoryService
            IJwtBearerAuthorization jwtBearerService = new JwtBearerAuthorization();
            var user = jwtBearerService.Validate(token, options);
            if (user == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
            context.User = user;
        }

        if (p.Accessibility == ProjectAccessibility.Private)
        {
            if (!context.Request.Headers.TryGetValue("X-Access-Key", out var accessKeys))
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }

            bool isValidAccess = false;
            for (int i = 0; i < accessKeys.Count; i++)
            {
                if (!string.IsNullOrEmpty(accessKeys[i]) && accessKeys[i] == p.PrivateKey)
                {
                    isValidAccess = true;
                    break;
                }
            }
            if (!isValidAccess)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                return;
            }
        }
        ICollection<AppRoute> routes;
        string key = p.Id.ToString();
        if (!await _cacheService.Exists(p.Id.ToString()))
        {
            var requests = await _requestRepository.GetProjectRequests(p.Id);
            routes = requests.Select(r => new AppRoute
            {
                Id = r.Id,
                Method = r.Method.ToLower(),
                Path = r.Path.ToLower()
            }).ToList();
            await _cacheService.Set(key, routes);
        }
        else
        {
            routes = await _cacheService.Get<ICollection<AppRoute>>(key);
        }
        var result = await _routeService.Resolve(req.Method.ToLower(), req.Path, routes);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"No matching route found for path: {req.Path}");
            return;
        }
        var r = await _requestRepository.Get(result.Route.Id);
        if (r.Type == RequestType.Fixed)
        {
            var @params = await _requestRepository.GetRequestParams(r.Id);
            foreach (var param in @params)
            {
                var value = context.Request.Query[param.Key].ToString() ?? "";
                if (!string.IsNullOrEmpty(param.Constraints))
                {
                    List<IConstraint> constraints = BuildConstraints(param.Constraints.Split(":").ToList());
                    foreach (var constraint in constraints)
                    {
                        if (!constraint.Match(value, out string message))
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync($"The '{param.Key}' parameter is not valid.");
                            await context.Response.WriteAsync(message);
                            return;
                        }
                    }
                }

                if (param.MatchExactly)
                {
                    if (context.Request.Query[param.Key].ToString() != param.Value)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The value of '{param.Key}' parameter is not valid.");
                        return;
                    }
                }
            }
            var headers = await _requestRepository.GetRequestHeaders(r.Id);
            foreach (var header in headers)
            {
                if (header.Required)
                {
                    if (!context.Request.Headers.ContainsKey(header.Name))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{header.Name}' header is required.");
                        return;
                    }
                    if (header.MatchExactly)
                    {
                        if (context.Request.Headers[header.Name].ToString() != header.Value)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            await context.Response.WriteAsync($"The value of '{header.Name}' header is not valid.");
                            return;
                        }
                    }
                }
            }
            var body = await _requestRepository.GetRequestBody(r.Id);
            if (body.Required)
            {
                context.Request.EnableBuffering();
                var text = await new StreamReader(context.Request.Body).ReadToEndAsync();
                context.Request.Body.Position = 0;
                if (string.IsNullOrEmpty(text))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is required.");
                    return;
                }
                if (body.MatchExactly)
                {
                    if (text != body.Text)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The body is not valid.");
                        return;
                    }
                }
            }
        }


        context.Items[nameof(AppRequest)] = new AppRequest()
        {
            Id = r.Id,
            Type = r.Type
        };
        context.Request.RouteValues = result.RouteValues;
        await next.Invoke(context);
    }

    private List<IConstraint> BuildConstraints(List<string> constraints)
    {
        var map = ConstraintBuilder.GetDefaultConstraintMap();
        var builder = new ConstraintBuilder(map);
        foreach (var constraintText in constraints)
        {
            builder.AddResolvedConstraint(constraintText);
        }
        return builder.Build();
    }

    private IncomingRequest GrabRequest(HttpRequest request)
    {
        var username = request.Host.Host.Split('.')[1];
        var projectName = request.Host.Host.Split('.')[0];
        var path = request.Path.Value.Remove(0, 1);
        return new IncomingRequest
        {
            Username = username,
            ProjectName = projectName,
            Method = request.Method,
            Path = path
        };
    }
}

public static class RequestValidatorExtensions
{
    public static IApplicationBuilder UseRequestValidator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestValidation>();
    }
}