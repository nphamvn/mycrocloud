using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Authorization;
using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Authorization;
using MycroCloud.WebMvc.Areas.Services.Services;
namespace MycroCloud.WebMvc.Areas.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureServicesArea(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IAuthorizationHandler, WebAppAuthorizationHandler>();
        static void options(GrpcClientFactoryOptions o)
        {
            o.Address = new Uri("https://localhost:5100");
        }
        services.AddGrpcClient<WebAppGrpcService.WebAppGrpcServiceClient>(options);
        services.AddGrpcClient<WebAppRouteGrpcService.WebAppRouteGrpcServiceClient>(options);
        services.AddGrpcClient<WebAppAuthenticationGrpcService.WebAppAuthenticationGrpcServiceClient>(options);
        services.AddScoped<IWebAppService, WebAppService>();
        services.AddScoped<IWebAppAuthenticationService, WebAppAuthenticationService>();
        services.AddScoped<IWebAppAuthorizationService, WebAppAuthorizationService>();
        services.AddScoped<IWebAppRouteService, WebAppRouteService>();
        return services;
    }
}
