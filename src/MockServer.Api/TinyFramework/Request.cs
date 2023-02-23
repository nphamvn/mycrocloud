using MockServer.Core.Enums;

namespace MockServer.Api.TinyFramework;

public class Request
{
    public int Id { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public RequestType Type { get; set; }
    public WebApplication WebApplication { get; set; }
    public HttpContext HttpContext { get; set; }
}
