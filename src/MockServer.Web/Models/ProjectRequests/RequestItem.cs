using MockServer.Core.Enums;

namespace MockServer.Web.Models.ProjectRequests;

public class RequestIndexItem : Request
{
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
