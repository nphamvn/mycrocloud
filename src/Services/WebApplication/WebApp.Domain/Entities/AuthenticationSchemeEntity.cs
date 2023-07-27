
using WebApp.Domain.Shared;

namespace WebApp.Domain.Entities;
public class AuthenticationSchemeEntity
{
    public int SchemeId { get; set; }
    public int WebAppId { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public AuthenticationSchemeType Type { get; set; }
    public int Order { get; set; }
    public AuthenticationSchemeOptions Options { get; set; }
    public string Description { get; set; }
}
public class AuthenticationSchemeOptions
{

}
public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string Header { get; set; }
    public List<ApiKey> Keys { get; set; } = new();
}
public class ApiKey
{
    public string Name { get; set; }
    public string Key { get; set; }
    public Dictionary<string, string> Payload { get; set; }
    public bool Active { get; set; } = true;
}
public class JwtBearerAuthenticationOptions : AuthenticationSchemeOptions
{
    public bool RequireHttpsMetadata { get; set; } = true;
    public string MetadataAddress { get; set; } = default!;
    public string SecretKey { get; set; }
    public IList<string> SymmetricSecuritySecretKeys { get; set; }
    public string Authority { get; set; }
    public string Algorithm { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string BinderSource { get; set; }
    public string TokenPrefix { get; set; }
    public string Description { get; set; }
}