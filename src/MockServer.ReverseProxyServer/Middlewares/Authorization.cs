using System.Security.Claims;
using System.Text.Json;
using Jint;
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
        if (auth.Title == "AllowAnonymous")
        {
            await next.Invoke(context);
            return;
        }
        else if (auth.Title == "Authorize")
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Response.StatusCode = 403; // Forbidden
                return;
            }

            if (!string.IsNullOrEmpty(auth.AuthenticationSchemes))
            {
                //allowedSchemes: "1,2,3,4,5"
                var allowedSchemes = auth.AuthenticationSchemes.Split(',');
                var authScheme = (string)context.Items["AuthSchemeId"];
                // check if the scheme is in the allowed list of scheme
                if (!allowedSchemes.Contains(authScheme))
                {
                    context.Response.StatusCode = 403; // Forbidden
                    return;
                }
            }

            if (!string.IsNullOrEmpty(auth.Expression))
            {
                var requirements = auth.Expression.Split(';');
                IAppAuthorizationService authorizationService = new AppAuthorizationService();
                foreach (var requirement in requirements)
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
    bool CheckRequirement(string expression, ClaimsPrincipal user);
}

public class AppAuthorizationService : IAppAuthorizationService
{
    private readonly Engine _engine;
    public AppAuthorizationService()
    {
        _engine = new Engine();
    }
    public bool CheckRequirement(string expression, ClaimsPrincipal user)
    {
        _engine.SetValue("User", JsonSerializer.Serialize(user));
        return Convert.ToBoolean(_engine.Execute(string.Format("const result = {0}", expression)).GetCompletionValue().ToString());
    }
}