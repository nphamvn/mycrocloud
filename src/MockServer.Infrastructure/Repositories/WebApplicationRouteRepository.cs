using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;

namespace MockServer.Infrastructure.Repositories;

public class WebApplicationRouteRepository : IWebApplicationRouteRepository
{
    private readonly string _connectionString;
    public WebApplicationRouteRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }

    public async Task<int> Create(int appId, Route route)
    {
        var query =
                """
                INSERT INTO 
                    WebApplicationRoute 
                    (
                        WebApplicationId,
                        IntegrationType,
                        Name,
                        Method,
                        Path,
                        Description
                    ) VALUES (
                        @WebApplicationId,
                        @IntegrationType,
                        @Name,
                        @Method,
                        @Path,
                        @Description
                    );
                SELECT last_insert_rowid();
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleAsync<int>(query, new
        {
            WebApplicationId = appId,
            IntegrationType = (int)route.IntegrationType,
            Name = route.Name,
            Method = route.Method,
            Path = route.Path,
            Description = route.Description
        });
    }

    public async Task Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var trans = await connection.BeginTransactionAsync();
        try
        {
            var delete = "DELETE FROM WebApplicationRoute WHERE Id = @Id;";
            await connection.ExecuteAsync(delete, new { Id = id }, trans);
            await trans.CommitAsync();
        }
        catch (Exception)
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<Route> Find(int appId, string method, string path)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.WebApplicationId,
                    r.IntegrationType,
                    r.Name,
                    r.Method,
                    r.Path,
                    r.Description,
                    r.Authorization
                FROM
                    WebApplicationRoute r
                INNER JOIN
                    WebApplication p ON r.WebApplicationId = p.Id AND r.WebApplicationId = @WebApplicationId
                WHERE
                    upper(r.method) = upper(@method) AND 
                    upper(r.Path) = upper(@path);
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Route>(query, new
        {
            WebApplicationId = appId,
            method = method,
            path = path
        });
    }

    public async Task<Route> GetById(int id)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.WebApplicationId,
                    r.IntegrationType,
                    r.Name,
                    r.Method,
                    r.Path,
                    r.Description,
                    r.Authorization,
                    r.RequestQueries,
                    r.RequestHeaders,
                    r.RequestBody
                FROM 
                    WebApplicationRoute r
                WHERE 
                    r.Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Authorization>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestQueryValidationItem>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestHeaderValidationItem>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestBodyValidationItem>>());
        return await connection.QuerySingleOrDefaultAsync<Route>(query, new
        {
            Id = id
        });
    }

    public async Task<IEnumerable<Route>> GetByApplicationId(int appId, string searchTerm, string sort)
    {
        var query =
                """
                SELECT 
                    r.Id,
                    r.IntegrationType,
                    r.Name,
                    r.Method,
                    r.Path
                FROM
                    WebApplicationRoute r
                WHERE
                    r.WebApplicationId = @WebApplicationId
                /**/
                """;
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query +=
                    """
                    AND r.Name LIKE @Query OR r.PATH LIKE @Query
                    """;
        }
        if (!string.IsNullOrEmpty(sort))
        {
            query +=
                """
                /**/
                ORDER BY r.Id
                """;
        }
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Route>(query, new
        {
            WebApplicationId = appId,
            Query =  "%" + searchTerm + "%"
        });
    }

    public async Task Update(int id, Route route)
    {
        var query =
                    """
                    UPDATE
                        WebApplicationRoute
                    SET
                        Name = @Name,
                        Method = @Method,
                        Path = @Path,
                        Description = @Description
                    WHERE
                        Id = @Id
                    """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = route.Name,
            Method = route.Method,
            Path = route.Path,
            Description = route.Description
        });
    }

    public Task AttachAuthorization(int id, Authorization authorization)
    {
        var query =
                """
                UPDATE 
                    WebApplicationRoute
                SET
                    Authorization = @Authorization
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<int>>());
        return connection.ExecuteAsync(query, new
        {
            Id = id
        });
    }

    public Task<Authorization> GetAuthorization(int id)
    {
        throw new NotImplementedException();
    }

    public Task<MockIntegration> GetMockIntegration(int routeId)
    {
        var query =
                """
                SELECT
                    mi.Code,
                    mi.ResponseHeaders,
                    mi.ResponseBodyText,
                    mi.ResponseBodyTextFormat,
                    mi.ResponseBodyTextRenderEngine,
                    mi.ResponseStatusCode,
                    mi.ResponseDelay,
                    mi.ResponseDelayTime
                FROM
                    WebApplicationRouteMockIntegration mi
                WHERE
                    RouteId = @RouteId
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<MockIntegrationResponseHeader>>());
        return connection.QuerySingleOrDefaultAsync<MockIntegration>(query, new
        {
            RouteId = routeId
        });
    }

    public Task UpdateMockIntegration(int id, MockIntegration integration)
    {
        var query =
                """
                UPDATE
                    WebApplicationRouteMockIntegration
                SET
                    Code = @Code,
                    ResponseHeaders = @ResponseHeaders,
                    ResponseBodyText = @ResponseBodyText,
                    ResponseBodyTextFormat = @ResponseBodyTextFormat,
                    ResponseBodyTextRenderEngine = @ResponseBodyTextRenderEngine,
                    ResponseStatusCode = @ResponseStatusCode,
                    ResponseDelay = @ResponseDelay,
                    ResponseDelayTime = @ResponseDelayTime
                WHERE
                    RouteId = @RouteId
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<MockIntegrationResponseHeader>>());
        return connection.QuerySingleOrDefaultAsync<MockIntegration>(query, new
        {
            RouteId = id,
            Code = integration.Code,
            ResponseHeaders = integration.ResponseHeaders,
            ResponseBodyText = integration.ResponseBodyText,
            ResponseBodyTextFormat = integration.ResponseBodyTextFormat,
            ResponseBodyTextRenderEngine = integration.ResponseBodyTextRenderEngine,
            ResponseStatusCode = integration.ResponseStatusCode,
            ResponseDelay = integration.ResponseDelay,
            ResponseDelayTime = integration.ResponseDelayTime
        });
    }

    public Task<DirectForwardingIntegration> GetDirectForwardingIntegration(int routeId)
    {
        var query =
                """
                SELECT
                    mi.ExternalServerHost
                FROM
                    WebApplicationRouteDirectForwardingIntegration mi
                WHERE
                    RouteId = @RouteId
                """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QuerySingleOrDefaultAsync<DirectForwardingIntegration>(query, new
        {
            RouteId = routeId
        });
    }

    public Task UpdateDirectForwardingIntegration(int id, DirectForwardingIntegration integration)
    {
        throw new NotImplementedException();
    }

    public Task<FunctionTriggerIntegration> GetFunctionTriggerIntegration(int id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateFunctionTriggerIntegration(int id, FunctionTriggerIntegration integration)
    {
        throw new NotImplementedException();
    }
}