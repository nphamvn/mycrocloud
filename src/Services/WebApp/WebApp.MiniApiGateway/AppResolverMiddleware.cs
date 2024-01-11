using WebApp.Domain.Repositories;

namespace WebApp.MiniApiGateway;

public class AppResolverMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IAppRepository appRepository)
    {
        int? appId = null;
        var fromHeader = true;
        if (fromHeader)
        {
            if (context.Request.Headers.TryGetValue("X-AppId", out var headerAppId)
                && int.TryParse(headerAppId.ToString()["App-".Length..], out var parsedAppId))
            {
                appId = parsedAppId;
            }
        }

        if (appId is null)
        {
            context.Response.StatusCode = 404;
            return;
        }
        var app = await appRepository.FindByAppId(appId.Value);
        if (app is null)
        {
            context.Response.StatusCode = 404;
            return;
        }
        context.Items["_App"] = app;
        await next(context);
    }
}

public static class AppResolverMiddlewareExtensions
{
    public static IApplicationBuilder UseAppResolverMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AppResolverMiddleware>();
    }
}