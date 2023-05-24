using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MockServer.Core.WebApplications.Security.ApiKey;

namespace MockServer.Api.TinyFramework;

public class ApiKeyAuthHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private readonly ApiKeyAuthenticationOptions options;
    public ApiKeyAuthHandler(ApiKeyAuthenticationOptions options)
    {
        this.options = options;
    }
    protected override Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(options.Header, out var value);
        var key = value.ToString();
        var keys = options.Keys;
        var found = keys.FirstOrDefault(k => k.Key == key);
        if (found is ApiKey apiKey)
        {
            var claims = new List<Claim>();
            foreach (var claim in apiKey.Payload)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }
            var claimsIdentity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(claimsIdentity);
            var ticket = new AuthenticationTicket(principal, nameof(ApiKeyAuthHandler));
            return Task.FromResult(((AuthenticateResult)AuthenticateResult.Success(ticket)));
        }
        else
        {
            return Task.FromResult(((AuthenticateResult)AuthenticateResult.Fail("Invalid key")));
        }
    }
}
