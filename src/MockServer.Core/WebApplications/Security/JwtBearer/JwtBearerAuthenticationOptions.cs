using Microsoft.IdentityModel.Tokens;

namespace MockServer.Core.WebApplications.Security.JwtBearer;

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
