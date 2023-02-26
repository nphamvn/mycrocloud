using MockServer.Core.WebApplications.Security;

namespace MockServer.Api.TinyFramework;

public static class AddAuthenticationSchemeExtentions
{
    public static WebApplication AddAuthenticationScheme(this WebApplication application,
            AuthenticationScheme scheme, IAuthenticationHandler handler)
    {
        application.AuthenticationSchemeHandlerMap.Add(scheme, handler);
        return application;
    }
}
