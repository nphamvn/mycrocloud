using MockServer.Core.Models.Auth;
using MockServer.Core.Services.Auth;

namespace MockServer.Core.Interfaces;

public interface IAuthenticationHandlerProvider {
    IAuthenticationHandler GetHandler(AuthenticationScheme scheme);
}