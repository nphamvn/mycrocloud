using System.Net;
using MockServer.Core.Repositories;

namespace MockServer.Api.Middlewares;

public class WebApplicationResolver : IMiddleware
{
    private readonly IProjectRepository _projectRepository;
    public WebApplicationResolver(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var host = context.Request.Host.Host;
        var username = host.Split('.')[0];
        var appName = host.Split('.')[1];

        var project = await _projectRepository.Find(username, appName);
        if (project == default)
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