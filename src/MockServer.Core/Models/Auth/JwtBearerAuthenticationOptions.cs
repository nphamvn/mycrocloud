using Microsoft.IdentityModel.Tokens;

namespace MockServer.Core.Models.Auth;
public class AppClaim
{
    public string Type { get; set; }
    public string Value { get; set; }
}
public class JwtBearerAuthenticationOptions : AuthenticationOptions
{
    public string SecretKey { get; set; }
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; set; }
    public bool ValidateLifetime { get; set; }
    public bool ValidateIssuer { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuerSigningKey { get; set; }
    public string BinderSource { get; set; }
    public IList<AppClaim> AdditionalClaims { get; set; } = new List<AppClaim>();
    public string Description { get; set; }
}
