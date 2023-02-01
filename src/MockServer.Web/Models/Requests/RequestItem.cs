using MockServer.Core.Enums;

namespace MockServer.Web.Models.Requests;

public class RequestIndexItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RequestType Type { get; set; }
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
