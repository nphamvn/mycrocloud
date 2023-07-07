using Microsoft.Extensions.Options;
using MockServer.Domain.Settings;

namespace MockServer.Domain.Repositories;
public class BaseRepository
{
    private readonly DatabaseOptions _options;
    public string ConnectionString => _options.ConnectionString;
    public BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        _options = databaseOptions.Value;
    }
}