using MockServer.Api.TinyFramework.DataBinding;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;

namespace MockServer.Api.TinyFramework;

public class AuthenticationMiddleware : IMiddleware
{
    private readonly IAuthRepository _authRepository;
    private readonly DataBinderProvider _modelBinderProvider;

    public AuthenticationMiddleware(
            IAuthRepository authRepository,
            DataBinderProvider modelBinderProvider)
    {
        _authRepository = authRepository;
        _modelBinderProvider = modelBinderProvider;
    }

    public async Task<MiddlewareInvokeResult> InvokeAsync(Request request)
    {
        var app = request.WebApplication;
        var context = request.HttpContext;
        var schemes = await _authRepository.GetProjectAuthenticationSchemes(app.Id);
        var defaultScheme = schemes.FirstOrDefault(a => a.Order == 1);
        IAuthenticationHandlerProvider handlerProvider = new AuthenticationHandlerProvider();
        IAuthenticationHandler handler;
        AuthenticationScheme auth;
        AuthenticateResult result;
        if (defaultScheme != null)
        {
            auth = await _authRepository.GetAuthenticationScheme(defaultScheme.Id, defaultScheme.Type);
            handler = handlerProvider.GetHandler(auth);
            result = await handler.AuthenticateAsync(context);
            if (result.Succeeded)
            {
                context.Items["AuthSchemeId"] = auth.Id;
                context.User = result.Ticket.Principal;
            }
            else
            {
                // Try to authenticate the user using each scheme
                var otherSchemes = schemes.Where(s => s.Id != defaultScheme.Id);
                foreach (var scheme in otherSchemes)
                {
                    handler = handlerProvider.GetHandler(scheme);
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
                auth = await _authRepository.GetAuthenticationScheme(scheme.Id, scheme.Type);
                handler = handlerProvider.GetHandler(auth);
                result = await handler.AuthenticateAsync(context);
                if (result.Succeeded)
                {
                    context.Items["AuthSchemeId"] = auth.Id;
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
