using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace MockServer.Core.Interfaces;

public class JwtOptions {
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
}
public interface IJwtBearerAuthorization {
    string GenerateToken(JwtOptions options);
    ClaimsPrincipal Validate(string token, JwtOptions options);
}