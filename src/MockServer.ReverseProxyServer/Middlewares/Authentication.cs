using System.Net;
using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;

namespace MockServer.ReverseProxyServer.Middlewares;

public class Authentication : IMiddleware
{
    private readonly IRequestRepository _requestRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly DataBinderProvider _modelBinderProvider;
    public Authentication(
            IRequestRepository requestRepository,
            IProjectRepository projectRepository,
            DataBinderProvider modelBinderProvider)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _modelBinderProvider = modelBinderProvider;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        ArgumentNullException.ThrowIfNull(context.Items["ProjectId"]);
        ArgumentNullException.ThrowIfNull(context.Items["RequestId"]);
        var projectId = Convert.ToInt32(context.Items["ProjectId"]);
        var requestId = Convert.ToInt32(context.Items["RequestId"]);

        //TODO: GET Authentication configuration
        var options = await _projectRepository.GetJwtHandlerConfiguration(projectId);
        var tokenSource = options.TokenBinderSource;
        var binder = _modelBinderProvider.GetBinder(tokenSource);
        var token = binder.Get(context) as string;
        if (string.IsNullOrEmpty(token))
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("No token found");
            return;
        }
        options.Claims = new Dictionary<string, string>();
        foreach (var config in options.AdditionalClaims)
        {
            var claimType = config.Name;
            var binderSource = config.Value;
            binder = _modelBinderProvider.GetBinder(binderSource);
            var value = binder.Get(context);
            if (value != null)
            {
                options.Claims[claimType] = value as string;
            }
        }
        //TODO: Create instance with FactoryService
        IJwtBearerAuthorization jwtBearerService = new JwtBearerAuthorization();
        var user = jwtBearerService.Validate(token, options);
        if (user == null)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsync("Invalid token");
            return;
        }
        context.User = user;
        await next.Invoke(context);
    }

    private void ApiKeyAuthorization (){
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