namespace MockServer.Api.TinyFramework;

public static class UseMiddlewareExtensions
{
    public static WebApplication UseMiddleware<T>(this WebApplication application) where T: IMiddleware {
        var instance = application.ServiceProvider.GetRequiredService<T>();
        application.UseMiddleware(instance);
        return application;
    }
}
