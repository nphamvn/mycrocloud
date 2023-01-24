using Microsoft.AspNetCore.Http;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public abstract class AppAuthenticationHandler<AuthOptions> : IAppAuthenticationHandler
{
    public Task<AppAuthenticateResult> AuthenticateAsync(HttpContext context)
    {
        return HandleAuthenticateAsync(context);
    }

    protected abstract Task<AppAuthenticateResult> HandleAuthenticateAsync(HttpContext context);
}
