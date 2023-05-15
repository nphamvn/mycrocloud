using MockServer.Core.Identity;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.WebApplications.Security;
using MockServer.Core.WebApplications.Security.ApiKey;
using MockServer.Core.WebApplications.Security.JwtBearer;
using CoreWebApplication = MockServer.Core.WebApplications.WebApplication;
namespace MockServer.Api.TinyFramework;

public class Host
{
    private readonly HttpContext _context;
    private readonly IServiceProvider _provider;
    private readonly IWebApplicationAuthenticationSchemeRepository _authRepository;
    private readonly IFactoryService _factoryService;

    public Host(IHttpContextAccessor contextAccessor,
            IServiceProvider provider,
            IWebApplicationAuthenticationSchemeRepository authRepository,
            IFactoryService factoryService)
    {
        _context = contextAccessor.HttpContext ?? throw new ArgumentNullException(nameof(_context));
        _provider = provider;
        _authRepository = authRepository;
        _factoryService = factoryService;
    }

    public async Task Run()
    {
        var coreApp = _context.Items[Constants.HttpContextItem.WebApplication] as CoreWebApplication;
        ArgumentNullException.ThrowIfNull(coreApp);
        var builder = WebApplication.CreateBuilder(_provider);
        var app = builder.Build(coreApp);
        app.User = new User
        {
            Id = coreApp.UserId
        };
        app.UseMiddleware<RoutingMiddleware>();
        if (coreApp.UseMiddlewares.Contains(nameof(AuthenticationMiddleware)))
        {
            var schemes = (await _authRepository.GetAll(coreApp.Id)).ToList();
            for (int i = 0; i < schemes.Count; i++)
            {
                var scheme = await _authRepository.Get(schemes[i].Id, schemes[i].Type);
                IAuthenticationHandler handler = default;
                if (scheme.Type == AuthenticationSchemeType.JwtBearer)
                {
                    handler = _factoryService.Create<JwtBearerAuthenticationHandler>((JwtBearerAuthenticationOptions)scheme.Options);
                }
                else if (scheme.Type == AuthenticationSchemeType.ApiKey)
                {
                    handler = _factoryService.Create<ApiKeyAuthHandler>((ApiKeyAuthenticationOptions)scheme.Options);
                }
                app.AddAuthenticationScheme(scheme, handler);
            }
            app.UseMiddleware<AuthenticationMiddleware>(app.AuthenticationSchemeHandlerMap);
        }

        if (coreApp.UseMiddlewares.Contains(nameof(RequestValidationMiddleware)))
        {
            app.UseMiddleware<RequestValidationMiddleware>();
        }

        await app.Handle(_context);
    }
}
