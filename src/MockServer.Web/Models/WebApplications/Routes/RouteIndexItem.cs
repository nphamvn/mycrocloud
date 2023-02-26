using MockServer.Core.WebApplications;

namespace MockServer.Web.Models.WebApplications.Routes;

public class RouteIndexItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RouteIntegrationType IntegrationType { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }    
    public string MethodBadgeColor
        => Method switch
        {
            "DELETE" => "bg-red",
            "POST" => "bg-orange",
            "PUT" => "bg-yellow",
            "GET" => "bg-green",
            _ => ""
        };
}
