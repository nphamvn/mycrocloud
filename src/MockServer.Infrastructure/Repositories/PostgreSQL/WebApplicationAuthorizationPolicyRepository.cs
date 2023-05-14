using Dapper;
using Npgsql;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications.Security;
using Microsoft.Extensions.Options;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class WebApplicationAuthorizationPolicyRepository : BaseRepository, IWebApplicationAuthorizationPolicyRepository
{
    public WebApplicationAuthorizationPolicyRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.QuerySingleOrDefaultAsync<Policy>(query, new
        {
            Id = policyId
        });
    }

    public async Task<IEnumerable<Policy>> GetAll(int appId)
    {
        var query =
                """
                SELECT
                    policy_id,
                    web_application_id,
                    name,
                    conditional_expression ConditionalExpressions
                FROM
                    web_application_authorization_policy
                WHERE
                    web_application_id = @web_application_id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return await connection.QueryAsync<Policy>(query, new
        {
            web_application_id = appId
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
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = policy.Name,
            ConditionalExpressions = policy.ConditionalExpressions
        });
    }
}
