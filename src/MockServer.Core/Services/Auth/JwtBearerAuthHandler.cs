using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public class JwtBearerAuthHandler : AppAuthenticationHandler<JwtBearerAuthenticationOptions>
{
    private readonly string token;
    private readonly JwtBearerAuthenticationOptions options;
    public JwtBearerAuthHandler(string token, JwtBearerAuthenticationOptions options)
    {
        this.token = token;
        this.options = options;
    }
    protected override Task<AppAuthenticateResult> HandleAuthenticateAsync()
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
        try
        {
            var principal = jwtHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var ticket = new AuthenticationTicket(principal, "AppJwtBearer");
            return Task.FromResult(((AppAuthenticateResult)AppAuthenticateResult.Success(ticket)));
        }
        catch (Exception)
        {
            return Task.FromResult<AppAuthenticateResult>((AppAuthenticateResult)AppAuthenticateResult.Fail("Invalid token"));
        }
    }
}
