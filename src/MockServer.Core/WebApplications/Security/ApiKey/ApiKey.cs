namespace MockServer.Core.WebApplications.Security.ApiKey;

public class ApiKey
{
    public string Name { get; set; }
    public string Key { get; set; }
    public Dictionary<string, string> Payload { get; set; }
    public bool Active { get; set; } = true;
}
