using System.Text.Json;
using Dapper;
using Npgsql;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Core.Settings;
using Microsoft.Extensions.Options;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class DatabaseRespository : BaseRepository, IDatabaseRepository
{

    public DatabaseRespository(IOptions<PostgresSettings> options) : base(options)
    {
    }

    public Task Add(string userId, Database db)
    {
        var query =
                """
                INSERT INTO
                    Database (
                        UserId,
                        Name,
                        Description
                    )
                VALUES (
                    @UserId,
                    @Name,
                    @Description
                )
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = db.Name,
            Description = db.Description
        });
    }

    public Task Delete(int id)
    {
        var query =
                """
                DELETE FROM
                    Database
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = id
        });
    }

    public Task<Database> FindByUserId(string userId, string name)
    {
        var query =
                """
                SELECT
                    db.Id,
                    db.UserId,
                    db.Name,
                    db.Description,
                    db.Data
                FROM
                    Database db
                WHERE
                    db.UserId = @UserId AND db.Name = @Name
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.QuerySingleOrDefaultAsync<Database>(query, new
        {
            UserId = userId,
            Name = name
        });
    }

    public Task<Database> FindByUsername(string username, string name)
    {
        var query =
                """
                SELECT
                    db.Id,
                    db.UserId,
                    db.Name,
                    db.Description,
                    db.Data
                FROM
                    Database db
                    INNER JOIN
                        Users u
                    ON db.UserId = u.Id
                WHERE
                    u.Username = @Username AND db.Name = @Name
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.QuerySingleOrDefaultAsync<Database>(query, new
        {
            Username = username,
            Name = name
        });
    }

    public async Task<Database> Get(int id)
    {
        var query =
                """
                SELECT
                    db.Id,
                    db.UserId,
                    db.Name,
                    db.Description,
                    db.Data
                FROM
                    Database db
                WHERE
                    db.Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<Database>(query, new
        {
            Id = id
        });
    }

    public Task<IEnumerable<Database>> GetAll(string userId)
    {
        var query =
                """
                SELECT
                    db.Id,
                    db.UserId,
                    db.Name,
                    db.Description,
                    db.Data
                FROM
                    Database db
                WHERE
                    db.UserId = @UserId
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.QueryAsync<Database>(query, new
        {
            UserId = userId
        });
    }

    public async Task<IEnumerable<Service>> GetDatabaseUsingService(int id)
    {
        var query =
                """
                SELECT
                    UsingService
                FROM
                    Database
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        var json =  await connection.QuerySingleOrDefaultAsync<string>(query, new
        {
            Id = id
        });

        return !string.IsNullOrEmpty(json) ?
            JsonSerializer.Deserialize<IEnumerable<Service>>(json) : null;
    }

    public Task Update(int id, Database db)
    {
        var query =
                """
                UPDATE
                    Database
                SET
                    Name = @Name,
                    Description = @Description
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = db.Name,
            Description = db.Description
        });
    }

    public Task UpdateData(int id, string data)
    {
        var query =
                """
                UPDATE
                    Database
                SET
                    Data = @Data
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            Data = data
        });
    }

    public Task UpdateDatabaseUsingService(int id, IList<Service> services)
    {
        var query =
                """
                UPDATE
                    Database
                SET
                    UsingService = @UsingService
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            UsingService = JsonSerializer.Serialize(services)
        });
    }
}