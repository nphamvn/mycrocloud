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
        throw new NotImplementedException();
    }

    public Task Delete(int policyId)
    {
        throw new NotImplementedException();
    }

    public Task<Policy> Get(int policyId)
    {
        throw new NotImplementedException();
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

    public Task Update(int appId, Policy policy)
    {
        throw new NotImplementedException();
    }
}
