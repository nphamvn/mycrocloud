using System.Security.Claims;
using System.Text.Json;
using Jint;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;

namespace MockServer.ReverseProxyServer.Middlewares;

public class Authorization : IMiddleware
{
    private readonly IAuthRepository _authRepository;

    public Authorization(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context.Items["RequestId"]);
        var requestId = Convert.ToInt32(context.Items["RequestId"]);
        var auth = await _authRepository.GetRequestAuthorization(requestId);
        //No authorization is set
        if (auth == null)
        {
            await next.Invoke(context);
            return;
        }

        //AllowAnonymous
        if (auth.Type == AuthorizationType.AllowAnonymous)
        {
            await next.Invoke(context);
            return;
        }
        else if (auth.Type == AuthorizationType.Authorize)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 403; // Forbidden
                return;
            }

            if (auth.AuthenticationSchemes.Count > 0)
            {
                //allowedSchemes: "1,2,3,4,5"
                var allowedSchemes = auth.AuthenticationSchemes;
                var authSchemeId = Convert.ToInt32(context.Items["AuthSchemeId"]);
                // check if the scheme is in the allowed list of scheme
                if (!allowedSchemes.Contains(authSchemeId))
                {
                    context.Response.StatusCode = 403; // Forbidden
                    return;
                }
            }

            if (auth.Requirements.Any())
            {
                IAppAuthorizationService authorizationService = new AppAuthorizationService();
                foreach (var requirement in auth.Requirements)
                {
                    //Hanlde requirement
                    if (!authorizationService.CheckRequirement(requirement, context.User))
                    {
                        context.Response.StatusCode = 403; // Forbidden
                        return;
                    }
                }
            }
        }
        await next.Invoke(context);
    }
}

public interface IAppAuthorizationService
{
    bool CheckRequirement(Requirement requirement, ClaimsPrincipal user);
}

public class AppAuthorizationService : IAppAuthorizationService
{
    private readonly Engine _engine;
    public AppAuthorizationService()
    {
        _engine = new Engine();
    }
    public bool CheckRequirement(Requirement requirement, ClaimsPrincipal user)
    {
        //TODO: Fix
        _engine.SetValue("User", JsonSerializer.Serialize(user));
        return Convert.ToBoolean(_engine.Execute(string.Format("const result = {0}", requirement.ConditionalExpression)).GetCompletionValue().ToString());
    }
}