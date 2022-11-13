namespace MockServer.ReverseProxyServer.Models;

public class AppRequest : HttpRequestMessage
{
    public string PrivateKey { get; set; }
    public bool IsPublic { get; set; }
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