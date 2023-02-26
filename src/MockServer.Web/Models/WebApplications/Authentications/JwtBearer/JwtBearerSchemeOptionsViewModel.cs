using Microsoft.IdentityModel.Tokens;

namespace MockServer.Web.Models.WebApplications.Authentications.JwtBearer;

public class JwtBearerSchemeOptionsViewModel
{
    public bool RequireHttpsMetadata { get; set; } = true;
    public string MetadataAddress { get; set; } = default!;
    public string SecretKey { get; set; }
    public IList<string> SymmetricSecuritySecretKeys { get; set; }
    public string Authority { get; set; }
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string BinderSource { get; set; } = "Authorization";
    public string TokenPrefix { get; set; } = "Bearer";
    public string Description { get; set; }
}
