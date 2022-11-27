using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Infrastructure.Repositories;
using MockServer.WebMVC.Services;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Extentions
{
    public static class ServiceCollectionExtentions
    {
        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var ConnectionString = configuration.GetConnectionString("SQLite");
            var settings = new GlobalSettings();
            settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
            services.AddSingleton<GlobalSettings>(s => settings);

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddHttpContextAccessor();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IRequestRepository, RequestRepository>();
            services.AddScoped<IProjectService, ProjectService>();
        }
    }
}