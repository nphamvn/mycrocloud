using System.Net;
using Microsoft.Extensions.Options;
using MockServer.Api.Options;
using MockServer.Core.Repositories;
namespace MockServer.Api.Middlewares;

public class WebApplicationResolver : IMiddleware
{
    private readonly IWebApplicationRepository _webApplicationRepository;
    private readonly VirtualHostOptions _virtualHostOptions;
    public WebApplicationResolver(
        IWebApplicationRepository webApplicationRepository,
        IOptions<VirtualHostOptions> virtualHostOptions)
    {
        _webApplicationRepository = webApplicationRepository;
        _virtualHostOptions = virtualHostOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if(!context.Request.Headers.TryGetValue(_virtualHostOptions.WebApplicationIdHeader, out var appId)) {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"not found");
            return;
        }
        if (!int.TryParse(appId.ToString().TrimStart('0'), out var id))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"not found");
            return;
        }
        var app = await _webApplicationRepository.Get(id);
        if (app == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Application '{appId}' not found");
            return;
        }
        if (app.Blocked)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Application '{appId}' not found");
            return;
        }
        if (!app.Enabled)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Application '{appId}' not found");
            return;
        }
        context.Items[HttpContextItemConstants.WebApplication] = app;
        await next.Invoke(context);
    }
}
public static class WebApplicationResolverExtensions
{
    public static IApplicationBuilder UseWebApplicationResolver(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<WebApplicationResolver>();
    }
}