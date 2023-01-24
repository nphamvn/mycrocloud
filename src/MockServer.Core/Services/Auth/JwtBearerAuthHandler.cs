using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public class JwtBearerAuthHandler : AppAuthenticationHandler<JwtBearerAuthenticationOptions>
{
    private readonly JwtBearerAuthenticationOptions options;
    public JwtBearerAuthHandler(JwtBearerAuthenticationOptions options)
    {
        this.options = options;
    }
    protected override Task<AppAuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(options.Header, out var value);
        const string Bearer = "Bearer";
        if (string.IsNullOrEmpty(value) && !value.ToString().StartsWith(Bearer))
        {
            return Task.FromResult((AppAuthenticateResult)AppAuthenticateResult.Fail("Invalid token"));
        }
        var token = value.ToString().Substring(Bearer.Length).Trim();
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
