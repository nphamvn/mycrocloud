using Dapper;
using Npgsql;
using WebApp.Domain.Repositories;
using WebApp.Domain.Settings;
using WebApp.Domain.WebApplication.Entities;
using Microsoft.Extensions.Options;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class WebAppAuthorizationPolicyRepository : BaseRepository, IWebAppAuthorizationPolicyRepository
{
    public WebAppAuthorizationPolicyRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public async Task Add(int appId, PolicyEntity policyEntity)
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
            name = policyEntity.Name,
            description = policyEntity.Description,
            claims = policyEntity.Claims
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

    public async Task<PolicyEntity> Get(int policyId)
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
        return await connection.QuerySingleOrDefaultAsync<PolicyEntity>(query, new
        {
            policy_id = policyId
        });
    }

    public async Task<IEnumerable<PolicyEntity>> GetAll(int appId)
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
        return await connection.QueryAsync<PolicyEntity>(query, new
        {
            web_application_id = appId
        });
    }

    public async Task Update(int id, PolicyEntity policyEntity)
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
            name = policyEntity.Name,
            description = policyEntity.Description,
            claims = policyEntity.Claims
        });
    }
}
