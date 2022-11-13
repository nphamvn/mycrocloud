namespace MockServer.ReverseProxyServer.Models;

public class FixedRequest : AppRequest
{
    public FixedRequest()
    {

    }
    public FixedRequest(HttpContext context) : base(context)
    {

    }
}