namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestQueryViewModel
{
    public string Key { get; set; }
    public string MatchExactlyValue { get; set; }
    public string ConstraintsText { get; set; }
    public List<string> Constraints { get; set; }
}
