using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
using MockServer.Core.Services;
using AuthenticationTicket = Microsoft.AspNetCore.Authentication.AuthenticationTicket;
namespace MockServer.Api.TinyFramework;

public class JwtBearerAuthHandler : AuthenticationHandler<JwtBearerAuthenticationOptions>
{
    private readonly JwtBearerAuthenticationOptions _options;
    public JwtBearerAuthHandler(JwtBearerAuthenticationOptions options)
    {
        _options = options;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(_options.BinderSource, out var value);
        if (string.IsNullOrEmpty(value))
        {
            return Task.FromResult((AuthenticateResult)AuthenticateResult.Fail("No token found"));
        }
        else if (!value.ToString().StartsWith(_options.TokenPrefix))
        {
            return Task.FromResult((AuthenticateResult)AuthenticateResult.Fail("No token found"));
        }

        var token = value.ToString().Substring(_options.TokenPrefix.Length).Trim();

        IJwtBearerTokenService service = new JwtBearerTokenService();
        try
        {
            var principal = service.ValidateToken(token, _options);
            var ticket = new AuthenticationTicket(principal, Scheme.SchemeName);
            return Task.FromResult(new AuthenticateResult()
            {
                Succeeded = true,
                Ticket = ticket
            });
        }
        catch (Exception)
        {
            return Task.FromResult<AuthenticateResult>((AuthenticateResult)AuthenticateResult.Fail("Invalid token"));
        }
    }
}
