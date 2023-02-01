namespace MockServer.Api.Models;

public class ForwardingRequest : Request
{
    public string Scheme { get; set; }
    public string Host { get; set; }
}