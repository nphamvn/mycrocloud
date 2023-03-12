using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Infrastructure.Repositories;

public class WebApplicationAuthorizationPolicyRepository : IWebApplicationAuthorizationPolicyRepository
{
    private readonly string _connectionString;
    public WebApplicationAuthorizationPolicyRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }

    public Task Add(int appId, Policy policy)
    {
        var query =
                """
                INSERT INTO 
                    WebApplicationAuthorizationPolicy (
                        WebApplicationId,
                        Name,
                        ConditionalExpressions
                    )
                    VALUES (
                        @WebApplicationId,
                        @Name,
                        @ConditionalExpressions
                    );
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.ExecuteAsync(query, new
        {
            WebApplicationId = appId,
            Name = policy.Name,
            ConditionalExpressions = policy.ConditionalExpressions,
        });
    }

    public Task Delete(int policyId)
    {
        var query =
                """
                DELETE FROM
                    WebApplicationAuthorizationPolicy
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.ExecuteAsync(query, new
        {
            Id = policyId
        });
    }

    public Task<Policy> Get(int policyId)
    {
        var query =
                """
                SELECT
                    Id,
                    WebApplicationId,
                    Name,
                    ConditionalExpressions
                FROM
                    WebApplicationAuthorizationPolicy
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.QuerySingleOrDefaultAsync<Policy>(query, new
        {
            Id = policyId
        });
    }

    public Task<IEnumerable<Policy>> GetAll(int appId)
    {
        var query =
                """
                SELECT 
                    Id,
                    WebApplicationId,
                    Name,
                    ConditionalExpressions
                FROM
                    WebApplicationAuthorizationPolicy p
                WHERE
                    WebApplicationId = @WebApplicationId
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.QueryAsync<Policy>(query, new
        {
            WebApplicationId = appId
        });
    }

    public Task Update(int id, Policy policy)
    {
        var query =
                """
                UPDATE
                    WebApplicationAuthorizationPolicy
                SET
                    Name = @Name,
                    ConditionalExpressions = @ConditionalExpressions
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = policy.Name,
            ConditionalExpressions = policy.ConditionalExpressions
        });
    }
}
