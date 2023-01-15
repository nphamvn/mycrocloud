using MockServer.ReverseProxyServer.Models;

namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRouteResolver
{
    Task<RouteResolveResult> Resolve(string method, string path, ICollection<AppRoute> routes);
}

public class RouteResolveResult
{
    public AppRoute Route { get; set; }
    public RouteValueDictionary RouteValues { get; set; } = new();
}

