namespace MockServer.Api.TinyFramework;

public interface IRouteResolver
{
    Task<RouteResolveResult> Resolve(string method, string path, ICollection<Route> routes);
}

public class RouteResolveResult
{
    public Route Route { get; set; }
    public RouteValueDictionary RouteValues { get; set; } = new();
}
