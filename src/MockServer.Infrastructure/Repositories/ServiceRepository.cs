using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Enums;
using MockServer.Core.Models.Services;
using MockServer.Core.Repositories;
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
                    WebApps
                WHERE 
                    UserId = @UserId
                UNION
                SELECT
                    {(int)ServiceType.UserPool} TYPE,
                    2 Id,
                    'User Pool 1' Name
                FROM
                    UserPools
                WHERE
                    UserId = @UserId
                UNION
                SELECT
                    {(int)ServiceType.Authorizer} TYPE,
                    3 Id,
                    'JWT Bearer Authorizer 1' Name
                FROM
                    Authorizers
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
