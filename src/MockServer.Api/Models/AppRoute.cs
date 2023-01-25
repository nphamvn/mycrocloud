namespace MockServer.ReverseProxyServer.Models;

public class AppRoute
{
    public int Id { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
}
