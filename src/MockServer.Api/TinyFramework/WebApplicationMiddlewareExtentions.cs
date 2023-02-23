namespace MockServer.Api.TinyFramework;

public static class WebApplicationMiddlewareExtentions
{
    public static WebApplication UseMiddleware(this WebApplication application, IMiddleware middleware) {

        return application;
    }
}
