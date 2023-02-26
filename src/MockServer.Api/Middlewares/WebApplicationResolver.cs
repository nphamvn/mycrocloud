using System.Net;
using Microsoft.Extensions.Options;
using MockServer.Api.Options;
using MockServer.Core.Repositories;

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
        var host = context.Request.Host.Host;
        var username = host.Split('.')[_virtualHostOptions.UsernameHostIndex];
        var appName = host.Split('.')[_virtualHostOptions.ApplicationNameHostIndex];

        var app = await _projectRepository.Find(username, appName);
        if (app == null || app.Blocked)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{appName}' project is not found");
            return;
        }
        context.Items[nameof(WebApplication)] = app;
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