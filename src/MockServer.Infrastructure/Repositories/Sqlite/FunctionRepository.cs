using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Functions;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories.Sqlite;
public class FunctionRepository : IFunctionRepository
{
    private readonly string _connectionString;
    public FunctionRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public Task<int> Add(string userId, Function function)
    {
        const string sql =
            """
            INSERT INTO
                Function(
                    UserId,
                    Name,
                    RuntimeId,
                    Description
                )
                VALUES (
                    @UserId,
                    @Name,
                    @RuntimeId,
                    @Description
                )
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(sql, new
        {
            UserId = userId,
            Name = function.Name,
            RuntimeId = function.RuntimeId,
            Description = function.Description
        }); 
    }

    public Task Delete(int id)
    {
        const string sql = 
            """
            DELETE FROM
                FUNCTION
            WHERE
                Id = @Id
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(sql, new {
            Id = id
        }); 
    }

    public Task<Function> Get(int id)
    {
        const string sql = 
            """
            SELECT 
                Id,
                Name,
                RuntimeId,
                Description
            FROM
                Function
            WHERE
                Id = @Id
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QuerySingleOrDefaultAsync<Function>(sql, new
        {
            Id = id
        }); 
    }

    public Task<IEnumerable<Function>> GetAll(string userId)
    {
        const string sql = 
            """
            SELECT 
                Id,
                Name,
                RuntimeId,
                Description
            FROM
                Function
            WHERE
                UserId = @UserId
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryAsync<Function>(sql, new
        {
            UserId = userId
        }); 
    }

    public Task<IEnumerable<Runtime>> GetAllRuntimes()
    {
        const string sql = 
            """
            SELECT 
                Id,
                Name,
                Description
            FROM
                FunctionRuntime;
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryAsync<Runtime>(sql); 
    }

    public Task Update(int id, Function function)
    {
        const string sql = 
            """
            UPDATE
                Function
            SET 
                Name = @Name,
                Description = @Description
            WHERE
                Id = @Id
            """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(sql, new {
            Id = id,
            Name = function.Name,
            Description = function.Description
        }); 
    }
}