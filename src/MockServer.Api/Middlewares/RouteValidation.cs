using System.Net;
using MockServer.Core.Repositories;
using MockServer.Api.Interfaces;
using MockServer.Api.Models;
using Route = MockServer.Api.Models.Route;

namespace MockServer.Api.Middlewares;
public class RouteValidation : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IRouteResolver _routeService;
    private readonly IProjectRepository _projectRepository;
    private readonly ICacheService _cacheService;
    public RouteValidation(
            IRequestRepository requestRepository,
            ICacheService cacheService,
            IRouteResolver routeService,
            IProjectRepository projectRepository)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _cacheService = cacheService;
        _routeService = routeService;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var req = GrabRequest(context.Request);
        var p = await _projectRepository.Get(req.Username, req.ProjectName);
        if (p == default)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"'{req.ProjectName}' project is not found");
            return;
        }
        ICollection<Route> routes;
        string key = p.Id.ToString();
        if (!await _cacheService.Exists(p.Id.ToString()))
        {
            var requests = await _requestRepository.GetProjectRequests(p.Id);
            routes = requests.Select(r => new Route
            {
                Id = r.Id,
                Method = r.Method.ToLower(),
                Path = r.Path.ToLower()
            }).ToList();
            await _cacheService.Set(key, routes);
        }
        else
        {
            routes = await _cacheService.Get<ICollection<Models.Route>>(key);
        }
        var result = await _routeService.Resolve(req.Method.ToLower(), req.Path, routes);
        if (result == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync($"No matching route found for path: {req.Path}");
            return;
        }
        context.Items["ProjectId"] = p.Id;
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
public static class RouteValidationExtensions
{
    public static IApplicationBuilder UseRouteValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RouteValidation>();
    }
}