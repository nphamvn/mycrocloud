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

    public async Task Delete(int policyId)
    {
        var query =
"""
DELETE FROM
    web_application_authorization_policy
WHERE
    policy_id = @policy_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(query, new
        {
            policy_id = policyId
        });
    }

    public async Task<Policy> Get(int policyId)
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
        return await connection.QuerySingleOrDefaultAsync<Policy>(query, new
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

    public async Task Update(int id, Policy policy)
    {
        var query =
"""
UPDATE
    web_application_authorization_policy
SET
    name = @name,
    description = @description,
    claims = @claims
WHERE
    policy_id = @policy_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string, string>>());
        await connection.ExecuteAsync(query, new
        {
            policy_id = id,
            name = policy.Name,
            description = policy.Description,
            claims = policy.Claims
        });
    }
}
