namespace MockServer.Web.Models;

public class Request
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public FixedRequest? FixedRequest { get; set; }
    public ForwardRequest? ForwardRequest { get; set; }
}

public class FixedRequest
{
    public int StatusCode { get; set; }
    public string? Body { get; set; }
}

public class ForwardRequest
{
    public string Host { get; set; }
    public string Scheme { get; set; }
}