using System.Net;
using Microsoft.Extensions.Options;
using MockServer.Api.Options;
using MockServer.Core.Extentions;
using MockServer.Core.Repositories;

namespace MockServer.Api.Middlewares;

public class WebApplicationResolver : IMiddleware
{
    private readonly IProjectRepository _projectRepository;
    private readonly VirtualHostOptions _virtualHostOptions;
    public WebApplicationResolver(IOptions<VirtualHostOptions> virtualHostOptions, 
        IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
        _virtualHostOptions = virtualHostOptions.Value;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var host = context.Request.Host.Host;
        var username = host.Split('.')[_virtualHostOptions.UsernameHostIndex];
        var appName = host.Split('.')[_virtualHostOptions.ApplicationNameHostIndex];

        var project = await _projectRepository.Find(username, appName);
        if (project == null || project.Blocked)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{appName}' project is not found");
            return;
        }
        project.UseMiddlewares = new()
        {
            nameof(MockServer.Api.TinyFramework.AuthenticationMiddleware),
            nameof(MockServer.Api.TinyFramework.ConstraintValidationMiddleware)
        };
        context.Items["Project"] = project;
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