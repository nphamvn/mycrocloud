namespace MockServer.Core.WebApplications.Security.ApiKey;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Header { get; set; }
    public List<ApiKey> Keys { get; set; } = new();
}
