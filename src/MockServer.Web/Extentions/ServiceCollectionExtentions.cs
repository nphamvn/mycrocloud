using MockServer.Core.Interfaces;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Settings;
using MockServer.Infrastructure.Repositories;
using MockServer.Web.Services;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var ConnectionString = configuration.GetConnectionString("SQLite");
            var settings = new GlobalSettings();
            settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
            services.AddSingleton<GlobalSettings>(s => settings);
            services.AddTransient<IApiKeyService, ApiKeyService>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpContextAccessor();
            services.AddScoped<IWebApplicationRepository, WebApplicationRepository>();
            services.AddScoped<IWebApplicationRouteRepository, WebApplicationRouteRepository>();
            services.AddScoped<IWebApplicationAuthenticationSchemeRepository, WebApplicationAuthenticationSchemeRepository>();
            services.AddScoped<IWebApplicationAuthorizationPolicyRepository, WebApplicationAuthorizationPolicyRepository>();
            services.AddScoped<IDatabaseRepository, DatabaseRespository>();
            services.AddScoped<IWebApplicationWebService, WebApplicationWebService>();
            services.AddScoped<IWebApplicationRouteWebService, WebApplicationRouteWebService>();
            services.AddScoped<IWebApplicationAuthenticationWebService, WebApplicationAuthenticationWebService>();
            services.AddScoped<IWebApplicationAuthorizationWebService, WebApplicationAuthorizationWebService>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<IDatabaseWebService, DatabaseWebService>();
        }
    }
}