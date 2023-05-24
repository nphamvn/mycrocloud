using Microsoft.AspNetCore.Authentication;
using MockServer.Core.Interfaces;
using MockServer.Core.Services;
using MockServer.Core.WebApplications.Security.JwtBearer;

namespace MockServer.Api.TinyFramework;

public class JwtBearerAuthenticationHandler : AuthenticationHandler<JwtBearerAuthenticationOptions>
{
    private readonly JwtBearerAuthenticationOptions _options;
    public JwtBearerAuthenticationHandler(JwtBearerAuthenticationOptions options)
    {
        _options = options;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(_options.BinderSource, out var value);
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult(AuthenticateResult.Fail("No token found"));
        }
        else if (!value.ToString().StartsWith(_options.TokenPrefix))
        {
            return Task.FromResult(AuthenticateResult.Fail("No token found"));
        }

        var token = value.ToString().Substring(_options.TokenPrefix.Length).Trim();

        IJwtBearerTokenService service = new JwtBearerTokenService();
        try
        {
            var principal = service.ValidateToken(token, _options);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token"));
        }
    }
}
