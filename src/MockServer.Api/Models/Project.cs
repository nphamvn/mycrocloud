namespace MockServer.ReverseProxyServer.Models;

public class Project
{
    public string Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool PublicAccess { get; set; }
    public string PrivateKey { get; set; }
}