using System.Net;
using MockServer.Core.Enums;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Api.Constraints;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;

namespace MockServer.Api.Middlewares;
public class ConstraintsValidation : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRouteResolver _routeService;
    private readonly IProjectRepository _projectRepository;
    private readonly ICacheService _cacheService;
    private readonly DataBinderProvider _modelBinderProvider;
    public ConstraintsValidation(
            IRequestRepository requestRepository,
            ICacheService cacheService,
            IRouteResolver routeService,
            IProjectRepository projectRepository,
            DataBinderProvider modelBinderProvider)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _cacheService = cacheService;
        _routeService = routeService;
        _modelBinderProvider = modelBinderProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context.Items["RequestId"]);
        int id = Convert.ToInt32(context.Items["RequestId"]);
        var r = await _requestRepository.GetById(id);
        var queries = r.Queries;
        var queryBinder = new FromQueryDataBinder();
        foreach (var param in queries)
        {
            queryBinder.Query = param.Key;
            var value = queryBinder.Get(context);
            if (!string.IsNullOrEmpty(param.Constraints))
            {
                var constraintsList = param.Constraints.Split(":").ToList();
                List<IConstraint> constraints = BuildConstraints(constraintsList);
                bool matchexactly = constraintsList.Contains("matchexactly");
                if (matchexactly)
                {
                    constraints.Add(MatchExactlyConstraint.Create(param.Value));
                }
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
        }
        var headers = r.Headers;
        var headerBinder = new FromHeaderDataBinder();
        foreach (var header in headers)
        {
            headerBinder.Name = header.Name;
            var value = headerBinder.Get(context);
            if (!string.IsNullOrEmpty(header.Constraints))
            {
                var constraintsList = header.Constraints.Split(":").ToList();
                bool matchexactly = constraintsList.Contains("matchexactly");
                List<IConstraint> constraints = BuildConstraints(constraintsList);
                if (matchexactly)
                {
                    constraints.Add(MatchExactlyConstraint.Create(header.Value));
                }
                foreach (var constraint in constraints)
                {
                    if (!constraint.Match(value, out string message))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{header.Name}' header is not valid.");
                        await context.Response.WriteAsync(message);
                        return;
                    }
                }
            }
        }
        var body = await _requestRepository.GetRequestBody(r.Id);
        var bodyBinder = new FromBodyDataBinder();
        var text = bodyBinder.Get(context);
        if (!string.IsNullOrEmpty(body.Constraints))
        {
            var constraintsList = body.Constraints.Split(":").ToList();
            bool matchexactly = constraintsList.Contains("matchexactly");
            List<IConstraint> constraints = BuildConstraints(constraintsList);
            if (matchexactly)
            {
                constraints.Add(MatchExactlyConstraint.Create(body.Text));
            }
            foreach (var constraint in constraints)
            {
                if (!constraint.Match(text, out string message))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await context.Response.WriteAsync($"The body is not valid.");
                    await context.Response.WriteAsync(message);
                    return;
                }
            }
        }

        context.Items[nameof(Request)] = new Request()
        {
            Id = r.Id,
            Type = r.Type
        };
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

public static class ConstraintsValidationExtensions
{
    public static IApplicationBuilder UseConstraintsValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ConstraintsValidation>();
    }
}