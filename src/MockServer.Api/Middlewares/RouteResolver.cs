using System.Net;
using MockServer.Core.Repositories;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;
using Route = MockServer.Api.Models.Route;

namespace MockServer.Api.Middlewares;
public class RouteResolver : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRouteResolver _routeResolver;
    private readonly IProjectRepository _projectRepository;
    private readonly ICacheService _cacheService;
    public RouteResolver(
            IRequestRepository requestRepository,
            ICacheService cacheService,
            IRouteResolver routeService,
            IProjectRepository projectRepository)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _cacheService = cacheService;
        _routeResolver = routeService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var req = GrabRequest(context.Request);
        var app = await _projectRepository.Find(req.Username, req.ProjectName);
        if (app == default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{req.ProjectName}' project is not found");
            return;
        }
        ICollection<Route> registeredRoutes;
        string key = app.Id.ToString();
        if (!await _cacheService.Exists(app.Id.ToString()))
        {
            var requests = await _requestRepository.GetByProjectId(app.Id);
            registeredRoutes = requests.Select(r => new Route
            {
                Id = r.Id,
                Method = r.Method.ToLower(),
                Path = r.Path.ToLower()
            }).ToList();
            await _cacheService.Set(key, registeredRoutes);
        }
        else
        {
            registeredRoutes = await _cacheService.Get<ICollection<Route>>(key);
        }
        var result = await _routeResolver.Resolve(req.Method.ToLower(), req.Path, registeredRoutes);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"No matching route found for path: {req.Path}");
            return;
        }
        context.Items["Username"] = req.Username;
        context.Items["ProjectId"] = app.Id;
        context.Items["RequestId"] = result.Route.Id;
        context.Request.RouteValues = result.RouteValues;
        await next.Invoke(context);
    }
    private IncomingRequest GrabRequest(HttpRequest request)
    {
        string url = $"{request.Scheme}://{request.Host}:{request.Host.Port ?? 80}";
        var username = request.Host.Host.Split('.')[1];
        var projectName = request.Host.Host.Split('.')[0];
        var path = request.Path.Value.Remove(0, 1);
        return new IncomingRequest
        {
            Username = username,
            ProjectName = projectName,
            Method = request.Method,
            Path = path
        };
    }
}
public static class RouteResolverExtensions
{
    public static IApplicationBuilder UseRouteResolver(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouteResolver>();
    }
}