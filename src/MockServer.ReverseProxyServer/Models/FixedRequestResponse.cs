namespace MockServer.ReverseProxyServer.Services;

public class FixedRequestResponse
{
    public int StatusCode { get; set; }
    public string Body { get; set; }
}