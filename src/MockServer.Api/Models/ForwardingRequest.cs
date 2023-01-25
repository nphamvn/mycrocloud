namespace MockServer.ReverseProxyServer.Models;

public class ForwardingRequest : AppRequest
{
    public string Scheme { get; set; }
    public string Host { get; set; }
    public ForwardingRequest()
    {

    }
    public ForwardingRequest(HttpContext context)
    {
        HttpContext = context;
    }
}