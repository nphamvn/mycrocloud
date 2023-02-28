using System.Net;
using MockServer.Api.Interfaces;
using MockServer.Core.Repositories;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
using CoreRoute = MockServer.Core.WebApplications.Route;

namespace MockServer.Api.TinyFramework;

public class RoutingMiddleware : IMiddleware
{
    private readonly ICacheService _cacheService;
    private readonly IWebApplicationRouteRepository _requestRepository;
    private readonly IRouteResolver _routeResolver;

    public RoutingMiddleware(ICacheService cacheService,
            IWebApplicationRouteRepository requestRepository,
            IRouteResolver routeResolver)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
        _routeResolver = routeResolver;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(HttpContext context)
    {   
        var app = context.Items[typeof(CoreWebApplication).Name] as CoreWebApplication;
        ArgumentNullException.ThrowIfNull(app);
        ICollection<Route> registeredRoutes;
        string key = app.Id.ToString();
        if (!await _cacheService.Exists(app.Id.ToString()))
        {
            var requests = await _requestRepository.GetByApplicationId(app.Id);
            registeredRoutes = requests.Select(r => new Route
            {
                Id = r.Id,
                Method = r.Method.ToLower(),
                RouteTemplate = r.Path.ToLower()
            }).ToList();
            await _cacheService.Set(key, registeredRoutes);
        }
        else
        {
            registeredRoutes = await _cacheService.Get<ICollection<Route>>(key);
        }
        var result = await _routeResolver.Resolve(context.Request.Method.ToLower(), context.Request.Path, registeredRoutes);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"No matching route found for {context.Request.Method} {context.Request.Path}");
            return MiddlewareInvokeResult.End;
        }
        context.Items[typeof(CoreRoute).Name] = await _requestRepository.GetById(result.Route.Id);
        context.Request.RouteValues = result.RouteValues;
        return MiddlewareInvokeResult.Next;
    }
}
