using System.Net;
using MockServer.Api.Interfaces;
using MockServer.Core.Repositories;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
using CoreRoute = MockServer.Core.WebApplications.Route;
using MockServer.Core.Services;

namespace MockServer.Api.TinyFramework;

public class RoutingMiddleware : IMiddleware
{
    private readonly ICacheService _cacheService;
    private readonly IWebApplicationRouteRepository _routeRepository;
    private readonly IFactoryService _factoryService;

    public RoutingMiddleware(ICacheService cacheService,
            IWebApplicationRouteRepository routeRepository,
            IFactoryService factoryService)
    {
        _cacheService = cacheService;
        _routeRepository = routeRepository;
        _factoryService = factoryService;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(HttpContext context)
    {   
        var app = context.Items[HttpContextItemConstants.WebApplication] as CoreWebApplication;
        ArgumentNullException.ThrowIfNull(app);
        ICollection<Route> registeredRoutes;
        string key = app.Id.ToString();
        if (!await _cacheService.Exists(app.Id.ToString()))
        {
            var routes = await _routeRepository.GetByApplicationId(app.Id, string.Empty, string.Empty);
            registeredRoutes = routes.Select(r => new Route
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
        var routeResolver = _factoryService.Create<TemplateParserMatcherRouteService>(registeredRoutes);
        var result = await routeResolver.Resolve(context.Request.Method.ToLower(), context.Request.Path);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"No matching route found for {context.Request.Method} {context.Request.Path}");
            return MiddlewareInvokeResult.End;
        }
        var route = await _routeRepository.GetById(result.Route.Id);
        context.Items[HttpContextItemConstants.Route] = route;
        context.Request.RouteValues = result.RouteValues;
        return MiddlewareInvokeResult.Next;
    }
}
