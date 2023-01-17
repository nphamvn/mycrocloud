using Microsoft.IdentityModel.Tokens;

namespace MockServer.Core.Models.Authorization;
public class JwtHandlerConfiguration {
    public string SecretKey { get; set; }
    public string Algorithm { get; set; } = SecurityAlgorithms.HmacSha256;
    public ICollection<ClaimConfiguration> ClaimConfigurations { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int Expire { get; set; }
    public bool RequireExpirationTime { get; internal set; }
    public bool ValidateLifetime { get; internal set; }
    public bool ValidateIssuer { get; internal set; }
    public bool ValidateAudience { get; internal set; }
    public bool ValidateIssuerSigningKey { get; internal set; }
    public string TokenSource { get; set; }
}
public class ClaimConfiguration
{
    public string Type { get; set; }
    public string BindingSource { get; set; }
}