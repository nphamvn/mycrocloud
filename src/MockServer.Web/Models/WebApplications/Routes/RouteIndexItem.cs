namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public RouteMatchViewModel Match { get; set; }
}