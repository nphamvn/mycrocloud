namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteRequestQueryViewModel
{
    public RouteRequestQueryViewModel()
    {
        Constraints = new() { "required", "int", "minlength(8)"};
    }
    public string Key { get; set; }
    public List<string> Constraints { get; set; }
}
