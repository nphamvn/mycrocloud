using Microsoft.AspNetCore.Http;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public interface IAppAuthenticationHandler
{
    Task<AppAuthenticateResult> AuthenticateAsync(HttpContext context);
}
