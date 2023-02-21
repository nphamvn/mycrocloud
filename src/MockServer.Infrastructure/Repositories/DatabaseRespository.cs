using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Databases;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories;

public class DatabaseRespository : IDatabaseRepository
{
    private readonly string _connectionString;
    public DatabaseRespository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public Task Add(int userId, Database db)
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
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = db.Name,
            Description = db.Description
        });
    }

    public Task<Database> Find(int userId, string name)
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
        using var connection = new SqliteConnection(_connectionString);
        return connection.QuerySingleOrDefaultAsync<Database>(query, new
        {
            UserId = userId,
            Name = name
        });
    }

    public Task<Database> Find(string username, string name)
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
        using var connection = new SqliteConnection(_connectionString);
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
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Database>(query, new
        {
            Id = id
        });
    }

    public Task<IEnumerable<Database>> GetAll(int userId)
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
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryAsync<Database>(query, new
        {
            UserId = userId
        });
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
        using var connection = new SqliteConnection(_connectionString);
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
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            Data = data
        });
    }
}
