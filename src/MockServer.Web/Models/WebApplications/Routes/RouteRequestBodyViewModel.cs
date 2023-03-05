namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestBodyViewModel
{
    public List<RouteRequestBodyFieldConstraint> FieldConstraints { get; set; }
    public List<string> Constraints { get; set; }
    public string Text { get; set; }
}
