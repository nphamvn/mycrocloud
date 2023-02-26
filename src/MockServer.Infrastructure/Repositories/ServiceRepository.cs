using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly string _connectionString;
    public ServiceRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public async Task<IEnumerable<Service>> GetServices(int userid)
    {
        var query =
                $"""
                SELECT
                    {(int)ServiceType.WebApp} Type,
                    Id,
                    Name
                FROM
                    Project
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
