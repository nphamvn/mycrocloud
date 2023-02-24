using MockServer.Core.Settings;

namespace MockServer.Api.Extentions;

public static class ServiceCollectionExtentions
{
    public static void AddGlobalSettings(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = new GlobalSettings();
        settings.Sqlite = new SqlSettings { ConnectionString = configuration.GetConnectionString("SQLite") };
        services.AddSingleton<GlobalSettings>(s => settings);
    }
}