using System.Net;
using Microsoft.Extensions.Options;
using MockServer.Api.Options;
using MockServer.Core.Repositories;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
namespace MockServer.Api.Middlewares;

public class WebApplicationResolver : IMiddleware
{
    private readonly IWebApplicationRepository _projectRepository;
    private readonly VirtualHostOptions _virtualHostOptions;
    public WebApplicationResolver(
        IWebApplicationRepository projectRepository,
        IOptions<VirtualHostOptions> virtualHostOptions)
    {
        _projectRepository = projectRepository;
        _virtualHostOptions = virtualHostOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        //var host = context.Request.Host.Host;
        //var username = host.Split('.')[_virtualHostOptions.UsernameHostIndex];
        //var appName = host.Split('.')[_virtualHostOptions.ApplicationNameHostIndex];
        //<username>-<>.api.npham.me
        //var app = await _projectRepository.FindByUsername(username, appName);
        const string AppIdHeader = "X-App-Id";
        if(!context.Request.Headers.TryGetValue(AppIdHeader, out var appId)) {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"not found");
            return;
        }
        var app = await _projectRepository.Get(int.Parse(appId));
        if (app == null || app.Blocked)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Application '{appId}' not found");
            return;
        }
        context.Items[typeof(CoreWebApplication).Name] = app;
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