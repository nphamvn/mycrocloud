using MockServer.Core.Services;
using MockServer.Core.Settings;

namespace MockServer.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddGlobalSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new GlobalSettings();
        settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
        settings.DatabaseProvider = configuration["DatabaseProvider"];
        services.AddSingleton<GlobalSettings>(s => settings);
    }

    public static void AddModelBinderProvider(this IServiceCollection services, Action<DataBinderProviderOptions> options)
    {
        services.Configure<DataBinderProviderOptions>(options);
        services.AddSingleton<DataBinderProvider>();
    }
}