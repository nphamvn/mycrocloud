using System.Net;
using MockServer.Core.Enums;
using MockServer.Core.Repositories;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Middlewares;

public class RequestValidation
{
    private readonly IRequestRepository _requestRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly RequestDelegate _next;

    public RequestValidation(RequestDelegate next,
            IRequestRepository requestRepository,
            IProjectRepository projectRepository)
    {
        _next = next;
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var req = GrabRequest(context.Request);

        var p = await _projectRepository.Get(req.Username, req.ProjectName);
        if (p == default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{req.ProjectName}' project is not found");
            return;
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

        var r = await _requestRepository.Get(p.Id, req.Method, req.Path);
        if (r == default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("NotFound");
            return;
        }

        if (r.Type == RequestType.Fixed)
        {
            var @params = await _requestRepository.GetRequestParams(r.Id);
            foreach (var param in @params)
            {
                if (param.Required)
                {
                    if (!context.Request.Query.ContainsKey(param.Key))
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync($"The '{param.Key}' parameter is required.");
                        return;
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

        //context.Items[nameof(IncomingRequest)] = req;
        context.Items[nameof(AppRequest)] = new AppRequest()
        {
            Id = r.Id,
            Type = r.Type
        };
        await _next.Invoke(context);
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