using System.Net;
using MockServer.Api.Interfaces;
using MockServer.Core.Repositories;

namespace MockServer.Api.TinyFramework;

public class RoutingMiddleware : IMiddleware
{
    private readonly ICacheService _cacheService;
    private readonly IRequestRepository _requestRepository;
    private readonly IRouteResolver _routeResolver;

    public RoutingMiddleware(ICacheService cacheService,
            IRequestRepository requestRepository,
            IRouteResolver routeResolver)
    {
        _cacheService = cacheService;
        _requestRepository = requestRepository;
        _routeResolver = routeResolver;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(Request request)
    {
        var context = request.HttpContext;
        ICollection<Route> registeredRoutes;
        var app = request.WebApplication;
        string key = app.Id.ToString();
        if (!await _cacheService.Exists(request.WebApplication.Id.ToString()))
        {
            var requests = await _requestRepository.GetByProjectId(app.Id);
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
            await context.Response.WriteAsync($"No matching route found for {request.Method.ToUpper()} {request.Path}");
            return MiddlewareInvokeResult.End;
        }
        context.Request.RouteValues = result.RouteValues;
        return MiddlewareInvokeResult.Next;
    }
}
