using System.Net;
using MockServer.Core.Repositories;
namespace MockServer.Api.Middlewares;

public class WebApplicationResolver : IMiddleware
{
    private readonly IWebApplicationRepository _webApplicationRepository;
    private readonly IConfiguration _configuration;
    public WebApplicationResolver(
        IWebApplicationRepository webApplicationRepository,
        IConfiguration configuration)
    {
        _webApplicationRepository = webApplicationRepository;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if(!context.Request.Headers.TryGetValue(_configuration["WebApplicationIdHeader"], out var appId)) {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"The requested resource was not found");
            return;
        }
        if (!int.TryParse(appId.ToString().TrimStart('0'), out var id))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Invalid request. The provided header value could not be processed");
            return;
        }
        var app = await _webApplicationRepository.Get(id);
        if (app == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"The requested application was not found");
            return;
        }
        if (app.Blocked)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Access denied. The requested resource is currently unavailable");
            return;
        }
        if (!app.Enabled)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"Access denied. The requested resource is currently unavailable");
            return;
        }
        context.Items[Constants.HttpContextItem.WebApplication] = app;
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