using MockServer.Core.Models.Auth;

namespace MockServer.Api.TinyFramework;

public abstract class AuthenticationHandler<AuthenticationSchemeOptions> : AuthenticationHandler
{

}
public abstract class AuthenticationHandler : IAuthenticationHandler
{
    public AuthenticationScheme Scheme { get; set; }
    public Task<AuthenticateResult> AuthenticateAsync(HttpContext context)
    {
        return HandleAuthenticateAsync(context);
    }

    protected abstract Task<AuthenticateResult> HandleAuthenticateAsync(HttpContext context);
}
