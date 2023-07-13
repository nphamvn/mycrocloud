using Microsoft.Extensions.Options;
using WebApplication.Domain.Settings;

namespace WebApplication.Domain.Repositories;
public class BaseRepository
{
    private readonly DatabaseOptions _options;
    public string ConnectionString => _options.ConnectionString;
    public BaseRepository(IOptions<DatabaseOptions> databaseOptions)
    {
        _options = databaseOptions.Value;
    }
}