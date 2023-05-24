using MockServer.Core.Services;

namespace MockServer.Api.TinyFramework;

public static class UseMiddlewareExtensions
{
    public static WebApplication UseMiddleware<T>(this WebApplication application) where T: IMiddleware {
        var instance = application.ServiceProvider.GetRequiredService<T>();
        application.UseMiddleware(instance);
        return application;
    }

    public static WebApplication UseMiddleware<T>(this WebApplication application, params object[] parameters) where T : IMiddleware
    {
        var factoryService = application.ServiceProvider.GetRequiredService<IFactoryService>();
        var instance = factoryService.Create<T>(parameters);
        application.UseMiddleware(instance);
        return application;
    }
}
