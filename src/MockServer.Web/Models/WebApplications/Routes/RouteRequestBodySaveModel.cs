namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestBodySaveModel
{
    public Dictionary<string, List<string>> Constraints { get; set; }
    public string Text { get; set; }
}
