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
        var incomingRequest = GetRequestInfo(context.Request);

        var request = await _requestRepository.FindRequest(incomingRequest.Username,
            incomingRequest.ProjectName, incomingRequest.Method, incomingRequest.Path);

        if (request != null)
        {
            incomingRequest.RequestType = request.Type;
            incomingRequest.Id = request.Id;
            var project = await _projectRepository.GetById(request.ProjectId);
            if (project.Accessibility == ProjectAccessibility.Private)
            {
                string privateKey = context.Request.Headers["PrivateKey"];
                if (!string.IsNullOrEmpty(privateKey) && privateKey == project.PrivateKey)
                {
                    context.Items[nameof(IncomingRequest)] = incomingRequest;
                    await _next.Invoke(context);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                }
            }
            else
            {
                context.Items[nameof(IncomingRequest)] = incomingRequest;
                await _next.Invoke(context);
            }
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("Request not found");
        }
    }

    private IncomingRequest GetRequestInfo(HttpRequest request)
    {
        //username
        return new IncomingRequest
        {
            Username = request.Host.Host.Split('.')[0],
            ProjectName = request.Path.Value.Split('/')[1],
            Method = request.Method,
            Path = request.Path.Value.Remove(0, request.Path.Value.Split('/')[1].Length + 2)
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