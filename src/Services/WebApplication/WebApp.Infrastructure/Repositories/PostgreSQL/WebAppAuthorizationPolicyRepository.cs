using Dapper;
using Npgsql;
using WebApp.Domain.Repositories;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class WebAppAuthorizationPolicyRepository(IOptions<PostgresDatabaseOptions> databaseOptions) : BaseRepository(databaseOptions), IWebAppAuthorizationPolicyRepository
{
    public async Task Add(int appId, AuthorizationPolicyEntity AuthorizationPolicyEntity)
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
            name = AuthorizationPolicyEntity.Name,
            description = AuthorizationPolicyEntity.Description,
            claims = AuthorizationPolicyEntity.Claims
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

    public async Task<AuthorizationPolicyEntity> Get(int policyId)
    {
        var query =
"""
SELECT
    policy_id PolicyId,
    web_application_id WebAppId,
    name Name,
    description Description,
    claims Claims
FROM
    web_application_authorization_policy
WHERE
    policy_id = @policy_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Claims>());
        return await connection.QuerySingleOrDefaultAsync<AuthorizationPolicyEntity>(query, new
        {
            policy_id = policyId
        });
    }

    public async Task<IEnumerable<AuthorizationPolicyEntity>> GetAll(int appId)
    {
        var query =
"""
SELECT
    policy_id PolicyId,
    web_application_id WebAppId,
    name Name,
    description Description,
    claims Claims
FROM
    web_application_authorization_policy
WHERE
    web_application_id = @web_application_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Claims>());
        return await connection.QueryAsync<AuthorizationPolicyEntity>(query, new
        {
            web_application_id = appId
        });
    }

    public async Task Update(int id, AuthorizationPolicyEntity AuthorizationPolicyEntity)
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
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Claims>());
        await connection.ExecuteAsync(query, new
        {
            policy_id = id,
            name = AuthorizationPolicyEntity.Name,
            description = AuthorizationPolicyEntity.Description,
            claims = AuthorizationPolicyEntity.Claims
        });
    }
}
