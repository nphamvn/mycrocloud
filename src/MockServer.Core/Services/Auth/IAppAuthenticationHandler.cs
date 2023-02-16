using Microsoft.AspNetCore.Http;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public interface IAuthenticationHandler
{
    Task<AuthenticateResult> AuthenticateAsync(HttpContext context);
}
