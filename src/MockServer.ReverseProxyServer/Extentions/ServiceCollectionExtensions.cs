using MockServer.Core.Services;
using MockServer.Core.Settings;

namespace MockServer.ReverseProxyServer.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddGlobalSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new GlobalSettings();
        settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
        services.AddSingleton<GlobalSettings>(s => settings);
    }

    public static void AddModelBinderProvider(this IServiceCollection services, Action<ModelBinderProviderOptions> options)
    {
        services.Configure<ModelBinderProviderOptions>(options);
        services.AddSingleton<ModelBinderProvider>();
    }
}