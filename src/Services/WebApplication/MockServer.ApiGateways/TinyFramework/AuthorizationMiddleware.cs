using MockServer.Core.Repositories;
using MockServer.Core.WebApplications.Security;
using CoreRoute = MockServer.Core.WebApplications.Route;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
namespace MockServer.Api.TinyFramework;

public class AuthorizationMiddleware : IMiddleware
{
    private readonly IWebApplicationAuthenticationSchemeRepository _authRepository;

    public AuthorizationMiddleware(IWebApplicationAuthenticationSchemeRepository authRepository)
    {
        _authRepository = authRepository;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(HttpContext context)
    {
        var route = context.Items[typeof(CoreRoute).Name] as CoreRoute;
        var app = context.Items[nameof(CoreWebApplication)] as CoreWebApplication;
        var authorization = route.Authorization;
        //No authorization is set
        if (authorization == null)
        {
            return MiddlewareInvokeResult.Next;
        }
        else if (authorization.Type == AuthorizationType.AllowAnonymous)
        {
            //AllowAnonymous
            return MiddlewareInvokeResult.Next;
        }

        if (context.User == null || !context.User.Identity.IsAuthenticated)
        {
            context.Response.StatusCode = 403; // Forbidden
            return MiddlewareInvokeResult.End;
        }

        if (authorization.AuthenticationSchemeIds.Count > 0)
        {
            //allowedSchemes: "1,2,3,4,5"
            var allowedSchemeIds = authorization.AuthenticationSchemeIds;
            var authSchemeId = Convert.ToInt32(context.Items["AuthSchemeId"]);
            // check if the scheme is in the allowed list of scheme
            if (!allowedSchemeIds.Contains(authSchemeId))
            {
                context.Response.StatusCode = 403; // Forbidden
                return MiddlewareInvokeResult.End;
            }
        }

        if (authorization.Policies.Any())
        {
            IAuthorizationService authorizationService = new AuthorizationService();
            foreach (var policy in authorization.Policies)
            {
                //Handle requirement
                if (!authorizationService.CheckRequirement(policy, context.User))
                {
                    context.Response.StatusCode = 403; // Forbidden
                    return MiddlewareInvokeResult.End;
                }
            }
        }
        return MiddlewareInvokeResult.Next;
    }
}
