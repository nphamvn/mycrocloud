using System.Net;
using System.Security.Claims;
using MockServer.Core.Interfaces;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;
using MockServer.Core.Services;

namespace MockServer.ReverseProxyServer.Middlewares;

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
        ArgumentNullException.ThrowIfNull(context.Items["RequestId"]);
        ClaimsPrincipal user = null;
        var projectId = Convert.ToInt32(context.Items["ProjectId"]);
        var requestId = Convert.ToInt32(context.Items["RequestId"]);
        var auths = (await _projectRepository.Get(projectId)).Authentications;
        var @default = auths.FirstOrDefault(a => a.Order == 0);
        if (@default is Core.Entities.Auth.Authentication auth)
        {
            if (@default.Type == Core.Enums.AuthType.JwtBearer)
            {
                var options = (await _authRepository.GetAs(projectId, Core.Enums.AuthType.JwtBearer)).Options as JwtBearerAuthenticationOptions;
                const string source = "header[Authorization]";
                var binder = _modelBinderProvider.GetBinder(source);
                var token = binder.Get(context) as string;
                if (string.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("No token found");
                    return;
                }
                const string Bearer = "Bearer";
                if (!token.StartsWith(Bearer))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
                token = token.Substring(Bearer.Length).Trim();

                foreach (var claim in options.AdditionalClaims)
                {
                    binder = _modelBinderProvider.GetBinder(claim.Value);
                    claim.Value = binder.Get(context) as string;
                }

                //TODO: Create instance with FactoryService
                IJwtBearerTokenService jwtBearerService = new JwtBearerAuthorization();
                var handler = jwtBearerService.BuildHandler(token, options);
                var result = await handler.AuthenticateAsync();
                if (!result.Succeeded)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsync("Invalid token");
                    return;
                }
                user = result.Ticket.Principal;
            }
            else if (@default.Type == Core.Enums.AuthType.ApiKey)
            {
                user = null;
            }
        }
        else
        {

        }
        context.User = user;
        await next.Invoke(context);
    }

    private void ApiKeyAuthorization()
    {
        // if (!context.Request.Headers.TryGetValue("X-Access-Key", out var accessKeys))
        //     {
        //         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //         await context.Response.WriteAsync("Unauthorized");
        //         return;
        //     }

        //     bool isValidAccess = false;
        //     for (int i = 0; i < accessKeys.Count; i++)
        //     {
        //         if (!string.IsNullOrEmpty(accessKeys[i]) && accessKeys[i] == p.PrivateKey)
        //         {
        //             isValidAccess = true;
        //             break;
        //         }
        //     }
        //     if (!isValidAccess)
        //     {
        //         context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //         await context.Response.WriteAsync("Unauthorized");
        //         return;
        //     }
    }
}

public static class AuthenticationExtensions
{
    public static IApplicationBuilder UseRequestAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Authentication>();
    }
}