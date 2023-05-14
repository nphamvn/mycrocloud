namespace MockServer.Api.Options;

public class VirtualHostOptions
{
    public bool Enabled { get; set; }
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string UsernameSource { get; set; }
    public int UsernameHostIndex { get; set; }
    public string ApplicationNameSource { get; set; }
    public int ApplicationNameHostIndex { get; set; }
    public string WebApplicationIdHeader { get; set; }
    public const string Section = "VirtualHost";
}
