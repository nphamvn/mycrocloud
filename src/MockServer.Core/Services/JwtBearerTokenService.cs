using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
namespace MockServer.Core.Services;
public class JwtBearerTokenService : IJwtBearerTokenService
{
    public string GenerateToken(JwtBearerAuthenticationOptions options, List<Claim> claims)
    {
        // Create a security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        // Create a signing credentials using the security key
        var signingCredentials = new SigningCredentials(securityKey, options.Algorithm);
        // Create a ClaimsIdentity object
        var claimsIdentity = new ClaimsIdentity();
        foreach (var claim in claims)
        {
            claimsIdentity.AddClaim(new Claim(claim.Type, claim.Value));
        }
        // Create a JwtSecurityToken object
        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claimsIdentity.Claims,
            expires: DateTime.Now.AddMinutes(options.Expire),
            signingCredentials: signingCredentials
        );
        // Use the JwtSecurityTokenHandler to encode the token and create the JWT string
        var jwtHandler = new JwtSecurityTokenHandler();
        return jwtHandler.WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token, JwtBearerAuthenticationOptions options)
    {
        var jwtHandler = new JwtSecurityTokenHandler();
        // Create a security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        // Create a validation parameters object
        var validationParameters = new TokenValidationParameters()
        {
            RequireExpirationTime = options.RequireExpirationTime,
            ValidateLifetime = options.ValidateLifetime,
            ValidateIssuer = options.ValidateIssuer,
            ValidateAudience = options.ValidateAudience,
            ValidateIssuerSigningKey = options.ValidateIssuerSigningKey,
            ValidIssuer = options.Issuer,
            ValidAudience = options.Audience,
            IssuerSigningKey = securityKey
        };
        return jwtHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
    }
}