using MockServer.Api.Services;
using MockServer.Core.Models;
using MockServer.Core.Models.Auth;
using MockServer.Core.Services;

namespace MockServer.Api.TinyFramework;

public class WebApplication : IWebApplication
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ApplicationUser User { get; set; }
    public List<AuthenticationScheme> Authentications { get; set; }
    public List<string> UseMiddlewares { get; set; }

    public ApplicationUser Owner { get; set; }
    private Dictionary<string, IMiddleware> _useMiddlewares = new();
    public IServiceProvider ServiceProvider { get; set; }
    public IFactoryService FactoryService { get; set; }
    public WebApplication()
    {
        
    }
    public WebApplication(IServiceProvider provider)
    {
        ServiceProvider = provider;
        FactoryService = ServiceProvider.GetRequiredService<IFactoryService>();
    }
    
    public static WebApplicationBuilder CreateBuilder(IServiceProvider provider){
        return new WebApplicationBuilder(provider);
    }

    public void UseMiddleware(IMiddleware middleware){
        _useMiddlewares.Add(middleware.GetType().Name , middleware);
    }

    public async Task Handle(Request request)
    {
        await BeforeHandle(request);

        MiddlewareInvokeResult result = default;
        IMiddleware middleware;
        if (_useMiddlewares.ContainsKey(nameof(RoutingMiddleware)))
        {
            middleware = _useMiddlewares[nameof(RoutingMiddleware)];
            result = await middleware.InvokeAsync(request);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }

        if (_useMiddlewares.ContainsKey(nameof(AuthenticationMiddleware)))
        {
            middleware = _useMiddlewares[nameof(AuthenticationMiddleware)];
            result = await middleware.InvokeAsync(request);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }

        if (_useMiddlewares.ContainsKey(nameof(ConstraintValidationMiddleware)))
        {
            middleware = _useMiddlewares[nameof(ConstraintValidationMiddleware)];
            result = await middleware.InvokeAsync(request);

            if (result != MiddlewareInvokeResult.Next)
            {
                return;
            }
        }


        var handler = ServiceProvider.GetRequiredService<RequestHandler>();
        await handler.Handle(request);

        await AfterHandle(request);
    }

    private async Task BeforeHandle(Request request) {
        
    }

    private async Task AfterHandle(Request request)
    {

    }
}
