using Microsoft.Extensions.Options;

namespace WebApp.Infrastructure.Repositories.PostgreSql;
public class BaseRepository(IOptions<PostgresDatabaseOptions> databaseOptions)
{
    private readonly PostgresDatabaseOptions _options = databaseOptions.Value;
    public string ConnectionString => _options.ConnectionString;
}