using MockServer.Core.Enums;

namespace MockServer.Api.Models;

public class Request
{
    public int Id { get; set; }
    public RequestType Type { get; set; }
    public string Path { get; set; }
    public HttpContext HttpContext { get; set; }
}