using MockServer.Core.Models.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Interfaces;

namespace MockServer.Core.Services.Auth;

public class AuthenticationHandlerProvider : IAuthenticationHandlerProvider
{
    public IAuthenticationHandler GetHandler(AuthenticationScheme scheme)
    {
        if (scheme.Type is AuthenticationType.JwtBearer)
        {
            var JwtBearerAuthHandler = new JwtBearerAuthHandler((JwtBearerAuthenticationOptions)scheme.Options);
            JwtBearerAuthHandler.Scheme = scheme;
            return JwtBearerAuthHandler;
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