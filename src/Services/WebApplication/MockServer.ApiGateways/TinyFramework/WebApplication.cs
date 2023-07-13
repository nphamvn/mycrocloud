using Jint;
using MockServer.Core.Identity;
using MockServer.Core.Services;
using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
namespace MockServer.Api.TinyFramework;

public class WebApplication : IWebApplication
{
    public Dictionary<AuthenticationScheme, IAuthenticationHandler> AuthenticationSchemeHandlerMap { get; set; } = new();
    public List<string> UseMiddlewares { get; set; } = new();
    private Dictionary<string, IMiddleware> _useMiddlewares = new();
    public IServiceProvider ServiceProvider { get; set; }
    public IFactoryService FactoryService { get; set; }
    public Func<HttpContext, Task> OnStartHandleRequest { get; set; } = (ctx) => Task.CompletedTask;
    public Func<HttpContext, Task> OnCompleteHandleRequest { get; set; } = (ctx) => Task.CompletedTask;
    //Used to be mapped with AutoMapper
    public WebApplication()
    {

    }
    //Used to create instance manually
    public WebApplication(IServiceProvider provider)
    {
        ServiceProvider = provider;
        FactoryService = ServiceProvider.GetRequiredService<IFactoryService>();
    }

    public static WebApplicationBuilder CreateBuilder(IServiceProvider provider)
    {
        return new WebApplicationBuilder(provider);
    }

    public void UseMiddleware(IMiddleware middleware)
    {
        _useMiddlewares.Add(middleware.GetType().Name, middleware);
    }

    public async Task Handle(HttpContext context)
    {
        await OnStartHandleRequest(context);
        MiddlewareInvokeResult result = default;
        IMiddleware middleware;
        if (_useMiddlewares.ContainsKey(nameof(RoutingMiddleware)))
        {
            middleware = _useMiddlewares[nameof(RoutingMiddleware)];
            result = await middleware.InvokeAsync(context);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }

        if (_useMiddlewares.ContainsKey(nameof(AuthenticationMiddleware)))
        {
            middleware = _useMiddlewares[nameof(AuthenticationMiddleware)];
            result = await middleware.InvokeAsync(context);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }

        if (_useMiddlewares.ContainsKey(nameof(RequestValidationMiddleware)))
        {
            middleware = _useMiddlewares[nameof(RequestValidationMiddleware)];
            result = await middleware.InvokeAsync(context);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }
        var app = context.Items[typeof(CoreWebApplication).Name] as CoreWebApplication;
        var route = context.Items[typeof(CoreRoute).Name] as CoreRoute;
        var factoryService = ServiceProvider.GetRequiredService<IFactoryService>();
        RequestHandler handler = default;
        if (route.IntegrationType == RouteIntegrationType.MockIntegration)
        {
            var engine = new Engine();
            handler = factoryService.Create<MockIntegrationJintHandler>(engine, app, route);
        }
        else if (route.IntegrationType == RouteIntegrationType.DirectForwarding)
        {
            handler = factoryService.Create<DirectForwardingIntegrationHandler>(route);
        }

        await handler.Handle(context);

        await OnCompleteHandleRequest(context);
    }
}
