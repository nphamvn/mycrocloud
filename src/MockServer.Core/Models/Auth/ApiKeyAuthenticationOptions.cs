namespace MockServer.Core.Models.Auth;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Header { get; set; }
    public List<ApiKey> Keys { get; set; } = new();
}
public class ApiKey
{
    public string Name { get; set; }
    public string Key { get; set; }
    public List<AppClaim> Claims { get; set; } = new();
    public bool Active { get; set; } = true;
}
