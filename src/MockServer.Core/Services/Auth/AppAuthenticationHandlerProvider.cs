using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;

namespace MockServer.Core.Services.Auth;

public class AppAuthenticationHandlerProvider : IAppAuthenticationHandlerProvider
{
    public IAppAuthenticationHandler GetHandler(AppAuthentication scheme)
    {
        if (scheme.Type is AuthenticationType.JwtBearer)
        {
            return new JwtBearerAuthHandler((JwtBearerAuthenticationOptions)scheme.Options);
        }
        else if (scheme.Type is AuthenticationType.ApiKey)
        {
            return new ApiKeyAuthHandler((ApiKeyAuthenticationOptions)scheme.Options);
        }
        else
        {
            return null;
        }
    }
}