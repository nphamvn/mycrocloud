using Dapper;
using Npgsql;
using MockServer.Domain.Repositories;
using MockServer.Domain.Services;
using MockServer.Domain.Settings;
using Microsoft.Extensions.Options;
using MockServer.Domain.Services.Entities;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class ServiceRepository : BaseRepository, IServiceRepository
{
    public ServiceRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
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

        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<Service>(query, new
        {
            UserId = userid
        });
    }
}
