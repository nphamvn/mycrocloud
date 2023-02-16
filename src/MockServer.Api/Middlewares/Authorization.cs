using System.Dynamic;
using System.Security.Claims;
using System.Text.Json;
using Jint;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;

namespace MockServer.Api.Middlewares;

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
        var authorization = await _authRepository.GetRequestAuthorization(requestId);
        //No authorization is set
        if (authorization == null)
        {
            await next.Invoke(context);
            return;
        }
        else if (authorization.Type == AuthorizationType.AllowAnonymous)
        {
            //AllowAnonymous
            await next.Invoke(context);
            return;
        }

        if (context.User == null || !context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = 403; // Forbidden
            return;
        }

        if (authorization.AuthenticationSchemes.Count > 0)
        {
            //allowedSchemes: "1,2,3,4,5"
            var allowedSchemes = authorization.AuthenticationSchemes;
            var authSchemeId = Convert.ToInt32(context.Items["AuthSchemeId"]);
            // check if the scheme is in the allowed list of scheme
            if (!allowedSchemes.Contains(authSchemeId))
            {
                context.Response.StatusCode = 403; // Forbidden
                return;
            }
        }

        if (authorization.Policies.Any())
        {
            IAppAuthorizationService authorizationService = new AppAuthorizationService();
            foreach (var policy in authorization.Policies)
            {
                //Handle requirement
                if (!authorizationService.CheckRequirement(policy, context.User))
                {
                    context.Response.StatusCode = 403; // Forbidden
                    return;
                }
            }
        }
        await next.Invoke(context);
    }
}
public static class AuthorizationnExtensions
{
    public static IApplicationBuilder UseAppAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<Authorization>();
    }
}
public interface IAppAuthorizationService
{
    bool CheckRequirement(Policy requirement, ClaimsPrincipal user);
}

public class AppAuthorizationService : IAppAuthorizationService
{
    private readonly Engine _engine;
    public AppAuthorizationService()
    {
        _engine = new Engine();
    }
    public bool CheckRequirement(Policy requirement, ClaimsPrincipal user)
    {
        var userDic = new Dictionary<string, object>();
        foreach (var claim in user.Claims)
        {
            userDic[claim.Type] = claim.Value;
        }
        _engine.SetValue("user", userDic);
        _engine.Execute($"let user = {JsonSerializer.Serialize(userDic)};");
        var result = _engine.Execute(string.Format("eval({0})", requirement.ConditionalExpression)).GetCompletionValue().AsBoolean();
        return result;
    }
}