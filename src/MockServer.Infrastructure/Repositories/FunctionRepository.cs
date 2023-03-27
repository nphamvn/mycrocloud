using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Functions;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories;
public class FunctionRepository : IFunctionRepository
{
    private readonly string _connectionString;
    public FunctionRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public Task<int> Add(int userId, Function function)
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
                    @RuntimeId
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
        throw new NotImplementedException();
    }

    public Task<Function> Get(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Function>> GetAll(int userId)
    {
        const string sql = 
            """
            
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

    public Task<IEnumerable<Runtime>> GetAvailableRuntimes()
    {
        throw new NotImplementedException();
    }

    public Task Update(int id, Function function)
    {
        throw new NotImplementedException();
    }
}