using MockServer.Core.Enums;

namespace MockServer.ReverseProxyServer.Models;

public class AppRequest : HttpRequestMessage
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }

    private HttpContext _httpContext;
    public HttpContext HttpContext
    {
        get { return _httpContext; }
        set { _httpContext = value; }
    }

    public AppRequest()
    {

    }
    public AppRequest(HttpContext context)
    {
        _httpContext = context;
    }
}