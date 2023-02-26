namespace MockServer.Api.TinyFramework;

using MockServer.Core.WebApplications.Security;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
public class AuthenticationMiddleware : IMiddleware
{
    private readonly Dictionary<AuthenticationScheme, IAuthenticationHandler> _schemeHandlerMap;

    public AuthenticationMiddleware(Dictionary<AuthenticationScheme, IAuthenticationHandler> schemeHandlerMap)
    {
        _schemeHandlerMap = schemeHandlerMap;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(HttpContext context)
    {
        var app = context.Items[typeof(CoreWebApplication).Name] as CoreWebApplication;
        var schemes = new List<AuthenticationScheme>(this._schemeHandlerMap.Keys);
        IAuthenticationHandler handler;
        AuthenticateResult result;
        if (schemes.Any(s => s.Order == 1))
        {
            var defaultScheme = schemes.First(s => s.Order == 1);
            handler = _schemeHandlerMap[defaultScheme];
            result = await handler.AuthenticateAsync(context);
            if (result.Succeeded)
            {
                context.Items["AuthSchemeId"] = defaultScheme.Id;
                context.User = result.Ticket.Principal;
            }
            else
            {
                // Try to authenticate the user using each scheme
                var otherSchemes = schemes.Where(s => s.Id != defaultScheme.Id);
                foreach (var scheme in otherSchemes)
                {
                    handler = _schemeHandlerMap[defaultScheme];
                    result = await handler.AuthenticateAsync(context);
                    if (result.Succeeded)
                    {
                        // The scheme successfully authenticated the user
                        context.User = result.Principal;
                        break;
                    }
                }
            }
        }
        else
        {
            foreach (var scheme in schemes)
            {
                handler = _schemeHandlerMap[scheme];
                result = await handler.AuthenticateAsync(context);
                if (result.Succeeded)
                {
                    context.Items["AuthSchemeId"] = scheme.Id;
                    context.User = result.Principal;
                    break;
                }
            }
        }
        if (context.User == null)
        {
            // No scheme succeeded in authenticating the user, return a 401 Unauthorized
            context.Response.StatusCode = 401;
            return MiddlewareInvokeResult.End;
        }
        return MiddlewareInvokeResult.Next;
    }
}