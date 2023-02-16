using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly ApiKeyAuthenticationOptions options;
    public ApiKeyAuthHandler(ApiKeyAuthenticationOptions options)
    {
        this.options = options;
    }
    protected override Task<Models.Auth.AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(options.Header, out var value);
        var key = value.ToString();
        var keys = options.Keys;
        var found = keys.FirstOrDefault(k => k.Key == key);
        if (found is ApiKey apiKey)
        {
            var claims = new List<Claim>();
            foreach (var claim in apiKey.Claims)
            {
                claims.Add(new Claim(claim.Type, claim.Value));
            }
            var claimsIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(principal, nameof(ApiKeyAuthHandler));
            return Task.FromResult(((Models.Auth.AuthenticateResult)Models.Auth.AuthenticateResult.Success(ticket)));
        }
        else
        {
            return Task.FromResult(((Models.Auth.AuthenticateResult)Models.Auth.AuthenticateResult.Fail("Invalid key")));
        }
    }
}
