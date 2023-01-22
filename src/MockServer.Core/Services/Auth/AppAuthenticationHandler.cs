using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public abstract class AppAuthenticationHandler<AuthOptions> : IAppAuthenticationHandler
{
    public Task<AppAuthenticateResult> AuthenticateAsync()
    {
        return HandleAuthenticateAsync();
    }

    protected abstract Task<AppAuthenticateResult> HandleAuthenticateAsync();
}
