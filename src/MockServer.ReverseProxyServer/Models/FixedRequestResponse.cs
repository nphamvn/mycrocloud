namespace MockServer.ReverseProxyServer.Services;

public class FixedRequestResponse
{
    public int StatusCode { get; set; }
    public string Body { get; set; }
    public IDictionary<string,string> Headers { get; set; }
}