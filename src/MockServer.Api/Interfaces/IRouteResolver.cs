using MockServer.Api.Models;

namespace MockServer.Api.Interfaces;

public interface IRouteResolver
{
    Task<RouteResolveResult> Resolve(string method, string path, ICollection<Models.Route> routes);
}

public class RouteResolveResult
{
    public Models.Route Route { get; set; }
    public RouteValueDictionary RouteValues { get; set; } = new();
}

