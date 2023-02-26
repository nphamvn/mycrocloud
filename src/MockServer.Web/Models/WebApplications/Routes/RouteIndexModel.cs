namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexModel
{
    public WebApplication WebApplication { get; set; }
    public IEnumerable<RouteIndexItem> Routes { get; set; }
}