using MockServer.Core.Models.Auth;

namespace MockServer.Api.TinyFramework;

public interface IAuthenticationHandlerProvider {
    IAuthenticationHandler GetHandler(AuthenticationScheme scheme);
}