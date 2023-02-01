using MockServer.Core.Models.Auth;
using MockServer.Core.Services.Auth;

namespace MockServer.Core.Interfaces;

public interface IAppAuthenticationHandlerProvider {
    IAppAuthenticationHandler GetHandler(AppAuthentication scheme);
}