using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services;
public class JwtBearerAuthorization : IJwtBearerAuthorization
{
    public string GenerateToken(JwtOptions options)
    {
        // Create a security key
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        // Create a signing credentials using the security key
        var signingCredentials = new SigningCredentials(securityKey, options.Algorithm);
        // Create a ClaimsIdentity object
        var claimsIdentity = new ClaimsIdentity();
        foreach (var claim in options.Claims)
        {
            claimsIdentity.AddClaim(new Claim(claim.Key, claim.Value));
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

    public ClaimsPrincipal Validate(string token, JwtOptions options)
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
        // Validate the token
        ClaimsPrincipal principal = null;
        try
        {
            principal = jwtHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
        }
        catch (SecurityTokenException ex)
        {
            Console.WriteLine("Invalid token: " + ex.Message);
        }
        if (principal != null)
        {
            Console.WriteLine("Token is valid!");
        }
        return principal;
    }
}