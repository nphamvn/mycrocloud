using System.Net;
using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Middlewares;

public class RequestValidator
{
    private readonly IRequestServices _requestService;
    private readonly RequestDelegate _next;

    public RequestValidator(RequestDelegate next, IRequestServices requestService)
    {
        _next = next;
        _requestService = requestService;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var requestModel = new RequestModel
        {
            Username = context.Request.Host.Host.Split('.')[0],
            Method = context.Request.Method,
            Path = context.Request.Path.Value.StartsWith('/') ? context.Request.Path.Value.Remove(0, 1) : context.Request.Path.Value
        };
        var request = await _requestService.FindRequest(requestModel);
        if (request != null)
        {
            if (request.Project.PublicAccess)
            {
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
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("Request not found");
        }
    }
}

public static class RequestValidatorExtensions
{
    public static IApplicationBuilder UseRequestValidator(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestValidator>();
    }
}