namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRouteService
{
    Task Map(int projectId);
    Task<RouteResolveResult> Resolve(string path, int projectId);
}

public class RouteResolveResult
{
    public string Name { get; set; }
    public int RequestId { get; set; }
    public RouteValueDictionary RouteValues { get; set; } = new();
}

