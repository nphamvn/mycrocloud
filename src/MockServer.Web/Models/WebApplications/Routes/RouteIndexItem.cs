using MockServer.Core.WebApplications;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexItem
{
    public int RouteId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Method { get; set; }
    public List<string> Methods { get; set; }
    public string Path { get; set; }
    public ResponseProvider ResponseProvider { get; set; }
}
