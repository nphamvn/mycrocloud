using MockServer.Core.Enums;
using MockServer.Core.Models.Projects;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MockServer.Web.Models.ProjectRequests;

public class RequestViewModel
{
    public int Id { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string Url => string.Format("https://{0}.{1}.mockserver.com:5000/{2}", Project.Name, Project.User.Username, Path);
    public AuthorizationConfiguration AuthorizationConfiguration { get; set; }
    public RequestConfiguration RequestConfiguration { get; set; }
    public HandlerConfiguration HandlerConfiguration { get; set; }
    public ResponseConfiguration ResponseConfiguration { get; set; }
    public Project Project { get; set; }
    public IEnumerable<SelectListItem> StatusCodeSelectListItem { get; set; }
    public string MethodTextColor
    => Method switch
    {
        "DELETE" => "text-red",
        "POST" => "text-orange",
        "PUT" => "text-yellow",
        "GET" => "text-green",
        _ => "text-red"
    };
}
