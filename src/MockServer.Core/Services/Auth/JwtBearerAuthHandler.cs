using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MockServer.Core.Interfaces;
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
        options.BinderSource = "Authorization";
        context.Request.Headers.TryGetValue(options.BinderSource, out var value);
        const string Bearer = "Bearer";
        if (string.IsNullOrEmpty(value) && !value.ToString().StartsWith(Bearer))
        {
            return Task.FromResult((AppAuthenticateResult)AppAuthenticateResult.Fail("Invalid token"));
        }
        var token = value.ToString().Substring(Bearer.Length).Trim();

        IJwtBearerTokenService service = new JwtBearerTokenService();
        try
        {
            var principal = service.ValidateToken(token, options);
            var ticket = new AuthenticationTicket(principal, "AppJwtBearer");
            return Task.FromResult(new AppAuthenticateResult()
            {
                Succeeded = true,
                Ticket = ticket
            });
        }
        catch (Exception)
        {
            return Task.FromResult<AppAuthenticateResult>((AppAuthenticateResult)AppAuthenticateResult.Fail("Invalid token"));
        }
    }
}
