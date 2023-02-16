using MockServer.Core.Models.Requests;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Models.Projects;

namespace MockServer.Web.Models.Requests;

public class RequestOpenViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public Authorization Authorization { get; set; }
    public string ProjectName { get; set; }
    public RequestType Type { get; set; }
    public string Username { get; set; }
    public string Description { get; set; }
    public string Url => string.Format("https://{0}.{1}.mockserver.com:5000/{2}", ProjectName, Username, Path);
    public string MethodTextColor
    => Method switch
    {
        "DELETE" => "text-red",
        "POST" => "text-orange",
        "PUT" => "text-yellow",
        "GET" => "text-green",
        _ => "text-red"
    };
    public RequestConfiguration Configuration { get; set; }
}
public class RequestConfiguration
{

}
public class FixedRequestConfiguration : RequestConfiguration
{
    public IList<RequestQuery> RequestParams { get; set; }
    public IList<RequestHeader> RequestHeaders { get; set; }
    public RequestBody RequestBody { get; set; }
    public IList<ResponseHeader> ResponseHeaders { get; set; }
    public Response Response { get; set; }
}
