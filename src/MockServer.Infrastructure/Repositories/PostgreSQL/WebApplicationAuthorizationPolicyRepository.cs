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

    public async Task Add(int appId, Policy policy)
    {
        var query =
                """
                INSERT INTO 
                    web_application_authorization_policy (
                        web_application_id,
                        "name",
                        description,
                        claims
                    )
                    VALUES (
                        @web_application_id,
                        @name,
                        @description,
                        @claims
                    );
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string, string>>());
        await connection.ExecuteAsync(query, new
        {
            web_application_id = appId,
            name = policy.Name,
            description = policy.Description,
            claims = policy.Claims
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
    policy_id PolicyId,
    web_application_id WebApplicationId,
    name Name,
    description Description,
    claims Claims
FROM
    web_application_authorization_policy
WHERE
    policy_id = @policy_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string, string>>());
        return connection.QuerySingleOrDefaultAsync<Policy>(query, new
        {
            policy_id = policyId
        });
    }

    public async Task<IEnumerable<Policy>> GetAll(int appId)
    {
        var query =
"""
SELECT
    policy_id PolicyId,
    web_application_id WebApplicationId,
    name Name,
    description Description,
    claims Claims
FROM
    web_application_authorization_policy
WHERE
    web_application_id = @web_application_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string, string>>());
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
