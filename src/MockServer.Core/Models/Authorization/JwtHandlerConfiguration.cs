using Microsoft.IdentityModel.Tokens;

namespace MockServer.Core.Models.Authorization;
public class JwtHandlerConfiguration {
    public string SecretKey { get; set; }
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public Dictionary<string, string> Claims { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; internal set; }
    public bool ValidateLifetime { get; internal set; }
    public bool ValidateIssuer { get; internal set; }
    public bool ValidateAudience { get; internal set; }
    public bool ValidateIssuerSigningKey { get; internal set; }
    public string TokenBinderSource { get; set; }
    public IList<ClaimOption> AdditionalClaims { get; set; } = new List<ClaimOption>();
}
public record ClaimOption(string Type, string Value);