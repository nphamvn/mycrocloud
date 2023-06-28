namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Match Match { get; set; }
}
public class Match {
    public int Order { get; set; }
    public ICollection<string> Methods { get; set; }
    public string Path { get; set; }
}