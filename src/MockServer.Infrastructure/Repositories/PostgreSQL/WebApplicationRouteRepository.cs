using Dapper;
using Npgsql;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications;
using MockServer.Core.WebApplications.Security;
using Microsoft.Extensions.Options;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class WebApplicationRouteRepository : BaseRepository, IWebApplicationRouteRepository
{
    public WebApplicationRouteRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public async Task<int> Create(int appId, Route route)
    {
        var query =
                """
                INSERT INTO web_application_route(
                    web_application_id,
                    name,
                    method,
                    path,
                    description)
                VALUES (
                    @web_application_id,
                    @name, 
                    @method,
                    @path, 
                    @description)
                RETURNING route_id;
                """;

        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleAsync<int>(query, new
        {
            web_application_id = appId,
            name = route.Name,
            method = route.Method,
            path = route.Path,
            description = route.Description
        });
    }

    public async Task Delete(int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
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
    route_id,
    web_application_id,
    integration_type,
    name,
    method,
    path,
    description,
    "authorization",
    request_query,
    request_header,
    request_body
FROM
    web_application_route
WHERE
    web_application_id = @web_application_id AND
    upper(method) = upper(@method) AND 
    upper(path) = upper(@path);
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<Route>(query, new
        {
            web_application_id = appId,
            method = method,
            path = path
        });
    }

    public async Task<Route> GetById(int id)
    {
        var query =
"""
SELECT
    route_id RouteId,
    web_application_id WebApplicationId,
    integration_type,
    name,
    method,
    path,
    description,
    "authorization",
    request_query,
    request_header,
    request_body
FROM
    web_application_route
WHERE 
    route_id = @route_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Authorization>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestQueryValidationItem>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestHeaderValidationItem>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestBodyValidationItem>>());
        return await connection.QuerySingleOrDefaultAsync<Route>(query, new
        {
            route_id = id
        });
    }

    public async Task<IEnumerable<Route>> GetByApplicationId(int appId, string searchTerm, string sort)
    {
        var query =
"""
SELECT 
    route_id RouteId,
    web_application_id,
    integration_type,
    "name",
    "method",
    "path",
    description,
    "authorization",
    request_query,
    request_header,
    request_body
FROM
    web_application_route
WHERE
    web_application_id = @web_application_id
/**/
""";
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query +=
                    """
                    AND "name" LIKE @query OR "path" LIKE @query
                    """;
        }
        if (!string.IsNullOrEmpty(sort))
        {
            query +=
                """
                /**/
                ORDER BY id
                """;
        }
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<Route>(query, new
        {
            web_application_id = appId,
            query =  "%" + searchTerm + "%"
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
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
        using var connection = new NpgsqlConnection(ConnectionString);
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