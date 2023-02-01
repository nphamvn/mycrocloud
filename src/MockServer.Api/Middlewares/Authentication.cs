using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Services.Auth;

namespace MockServer.Api.Middlewares;

public class Authentication : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IAuthRepository _authRepository;
    private readonly DataBinderProvider _modelBinderProvider;

    public Authentication(
            IRequestRepository requestRepository,
            IProjectRepository projectRepository,
            IAuthRepository authRepository,
            DataBinderProvider modelBinderProvider)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _authRepository = authRepository;
        _modelBinderProvider = modelBinderProvider;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context.Items["ProjectId"]);
        var projectId = Convert.ToInt32(context.Items["ProjectId"]);
        var schemes = await _authRepository.GetProjectAuthenticationSchemes(projectId);
        var defaultScheme = schemes.FirstOrDefault(a => a.Order == 1);
        IAppAuthenticationHandlerProvider handlerProvider = new AppAuthenticationHandlerProvider();
        IAppAuthenticationHandler handler;
        AppAuthenticateResult result;
        if (defaultScheme is Core.Models.Auth.AppAuthentication auth)
        {
            handler = handlerProvider.GetHandler(auth);
            //TODO: Modify header value
            result = await handler.AuthenticateAsync(context);
            if (result.Succeeded)
            {
                context.User = result.Ticket.Principal;
            }
            else
            {
                // Try to authenticate the user using each scheme
                var otherSchemes = schemes.Where(s => s.Id != defaultScheme.Id);
                foreach (var scheme in otherSchemes)
                {
                    handler = handlerProvider.GetHandler(scheme);
                    //TODO: Modify header value
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
                handler = handlerProvider.GetHandler(scheme);
                //TODO: Modify header value
                result = await handler.AuthenticateAsync(context);
                if (result.Succeeded)
                {
                    // The scheme successfully authenticated the user
                    context.User = result.Principal;
                    break;
                }
            }
        }
        if (context.User == null)
        {
            // No scheme succeeded in authenticating the user, return a 401 Unauthorized
            context.Response.StatusCode = 401;
            return;
        }
        await next.Invoke(context);
    }
}

public static class AuthenticationExtensions
{
    public static IApplicationBuilder UseRequestAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Authentication>();
    }
}