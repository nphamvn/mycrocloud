using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
using MockServer.Core.Services.Auth;

namespace MockServer.Core.Services;
public class JwtBearerAuthorization : IJwtBearerTokenService
{
    public JwtBearerAuthHandler BuildHandler(string token, JwtBearerAuthenticationOptions options)
    {
        return new JwtBearerAuthHandler(token, options);
    }

    public string GenerateToken(JwtBearerAuthenticationOptions options)
    {
        // Create a security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        // Create a signing credentials using the security key
        var signingCredentials = new SigningCredentials(securityKey, options.Algorithm);
        // Create a ClaimsIdentity object
        var claimsIdentity = new ClaimsIdentity();
        foreach (var claim in options.AdditionalClaims)
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
}