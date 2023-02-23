using MockServer.Core.Models.Auth;

namespace MockServer.Api.TinyFramework;

public interface IAuthenticationHandler
{
    Task<AuthenticateResult> AuthenticateAsync(HttpContext context);
}
