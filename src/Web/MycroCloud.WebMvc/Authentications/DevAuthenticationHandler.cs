
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MycroCloud.WebMvc.Authentications;

public class DevAuthenticationHandler(IOptionsMonitor<DevAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder)
    : AuthenticationHandler<DevAuthenticationOptions>(options, logger, encoder)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.Header, out var key) || string.IsNullOrEmpty(key))
        {
            return AuthenticateResult.Fail("Header not found");
        }
        if (!Options.KeyClaims.TryGetValue(key, out var claims))
        {
            return AuthenticateResult.Fail("Invalid key");
        }
        var identity = new ClaimsIdentity(claims, "Dev");
        var principal = new ClaimsPrincipal(identity);
        var tiket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(tiket);
    }
}