namespace MockServer.ReverseProxyServer.Interfaces;

public interface IRouteService
{
    Task Map(int projectId);
    Task<RouteResolveResult> Resolve(string path, int projectId);
}

public class RouteResolveResult
{
    public int RequestId { get; set; }
    public RouteValues RouteValues { get; set; } = new();
}

public class RouteValues : RouteValueDictionary
{

}

