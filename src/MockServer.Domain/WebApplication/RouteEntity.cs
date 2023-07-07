using MockServer.Domain.WebApplication.Entities;

namespace MockServer.Domain.WebApplication.Route;
public class RouteEntity
{
    public int RouteId { get; set; }
    public int WebApplicationId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RouteMatch Match { get; set; }
    public AuthorizationEntity Authorization { get; set; }
    public RouteValidation Validation { get; set; }
    public RouteResponse Response { get; set; }
    public WebApplicationEntity WebApplicationEntity { get; set; }
}