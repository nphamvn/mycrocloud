using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories.Sqlite;

public class ServiceRepository : IServiceRepository
{
    private readonly string _connectionString;
    public ServiceRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public async Task<IEnumerable<Service>> GetServices(string userid)
    {
        var query =
                $"""
                SELECT
                    {(int)ServiceType.WebApplication} Type,
                    Id,
                    Name
                FROM
                    WebApplication
                WHERE 
                    UserId = @UserId
                UNION
                SELECT
                    {(int)ServiceType.Database} Type,
                    Id,
                    Name
                FROM
                    Database
                WHERE
                    UserId = @UserId
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Service>(query, new
        {
            UserId = userid
        });
    }
}
