using Route = WebApp.Domain.Entities.Route;
namespace WebApp.Api.Models;

public class RouteCreateUpdateRequest
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public int ResponseStatusCode { get; set; }
    public string ResponseText { get; set; }
    public Route ToEntity()
    {
        return new Route() {
            Name = Name,
            Method = Method,
            Path = Path,
            ResponseStatusCode = ResponseStatusCode,
            ResponseText = ResponseText
        };
    }
    public void ToEntity(Route route)
    {
        route.Name = Name;
        route.Method = Method;
        route.Path = Path;
        route.ResponseStatusCode = ResponseStatusCode;
        route.ResponseText = ResponseText;
    }
}