using Microsoft.Extensions.Options;

namespace WebApp.Infrastructure.Repositories.PostgreSql;
public class BaseRepository
{
    private readonly DatabaseOptions _options;
    public string ConnectionString => _options.ConnectionString;
    public BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        _options = databaseOptions.Value;
    }
}