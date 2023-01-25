using MockServer.Core.Enums;

namespace MockServer.ReverseProxyServer.Models;

public class AppRequest
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }

    public HttpContext HttpContext { get; set; }
}