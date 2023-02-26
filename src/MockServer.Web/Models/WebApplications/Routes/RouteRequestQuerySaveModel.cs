namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestQuerySaveModel
{
    public string Key { get; set; }
    public string MatchExactlyValue { get; set; }
    public string ConstraintsText { get; set; }
    public List<string> Constraints { get; set; }
}
