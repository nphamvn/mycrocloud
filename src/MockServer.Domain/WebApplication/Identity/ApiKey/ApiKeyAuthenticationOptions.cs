namespace MockServer.Domain.WebApplication.Entities;

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Header { get; set; }
    public List<ApiKey> Keys { get; set; } = new();
}
