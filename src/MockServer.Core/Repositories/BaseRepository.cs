using Microsoft.Extensions.Options;
using MockServer.Core.Settings;

namespace MockServer.Core.Repositories;
public class BaseRepository {
    private readonly DatabaseOptions _options;
    public string ConnectionString => _options.ConnectionString;
    public BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        _options = databaseOptions.Value;
    }
}