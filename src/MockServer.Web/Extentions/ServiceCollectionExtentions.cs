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
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IProjectWebService, ProjectWebService>();
            services.AddScoped<IProjectSettingsWebService, ProjectSettingsWebService>();
            services.AddScoped<IRequestWebService, RequestWebService>();
        }
    }
}