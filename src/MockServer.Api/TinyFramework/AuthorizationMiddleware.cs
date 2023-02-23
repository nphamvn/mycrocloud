using MockServer.Core.Enums;
using MockServer.Core.Repositories;

namespace MockServer.Api.TinyFramework;

public class AuthorizationMiddleware : IMiddleware
{
    private readonly IAuthRepository _authRepository;

    public AuthorizationMiddleware(IAuthRepository authRepository)
    {
        _authRepository = authRepository;
    }
    public async Task<MiddlewareInvokeResult> InvokeAsync(Request request)
    {
        var authorization = await _authRepository.GetRequestAuthorization(request.Id);
        var context = request.HttpContext;
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

        if (authorization.AuthenticationSchemes.Count > 0)
        {
            //allowedSchemes: "1,2,3,4,5"
            var allowedSchemes = authorization.AuthenticationSchemes;
            var authSchemeId = Convert.ToInt32(context.Items["AuthSchemeId"]);
            // check if the scheme is in the allowed list of scheme
            if (!allowedSchemes.Contains(authSchemeId))
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
