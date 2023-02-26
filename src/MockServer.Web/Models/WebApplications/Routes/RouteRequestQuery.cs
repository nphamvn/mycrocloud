namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestQuery
{
    public string Key { get; set; }
    public string MatchExactlyValue { get; set; }
    public List<string> Constraints { get; set; }
}
