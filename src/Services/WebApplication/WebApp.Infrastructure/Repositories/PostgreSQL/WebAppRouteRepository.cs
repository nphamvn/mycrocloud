using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using WebApp.Domain.Repositories;
using WebApp.Domain.Settings;
using WebApp.Domain.WebApplication.Entities;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class WebAppRouteRepository : BaseRepository, IWebAppRouteRepository
{
    public WebAppRouteRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public async Task<int> Create(int appId, RouteEntity route)
    {
        const string query = """
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

        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleAsync<int>(query, new
        {
            web_application_id = appId,
            name = route.Name,
            //method = route.Method,
            //path = route.Path,
            description = route.Description
        });
    }

    public async Task Delete(int id)
    {
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var trans = await connection.BeginTransactionAsync();
        try
        {
            const string delete = "DELETE FROM WebApplicationRoute WHERE Id = @Id;";
            await connection.ExecuteAsync(delete, new { Id = id }, trans);
            await trans.CommitAsync();
        }
        catch (Exception)
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<RouteEntity> Find(int appId, string method, string path)
    {
        const string query = """
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
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<RouteEntity>(query, new
        {
            web_application_id = appId,
            method = method,
            path = path
        });
    }
    
    public async Task<RouteEntity> Get(int id)
    {
        const string query = """
SELECT
    route_id RouteId,
    web_application_id WebAppId,
    name Name,
    description Description,
    match Match,
    "authorization" Authorization,
    validation Validation,
    response Response
FROM
    web_application_route
WHERE 
    route_id = @route_id
""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<RouteMatch>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Authorization>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<RouteValidation>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<RouteResponse>());
        return await connection.QuerySingleOrDefaultAsync<RouteEntity>(query, new
        {
            route_id = id
        });
    }

    public async Task<IEnumerable<RouteEntity>> GetByApplicationId(int appId, string searchTerm, string sort)
    {
        var query =
"""
SELECT 
    route_id RouteId,
    web_application_id WebAppId,
    "name" Name,
    description Description
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

        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<RouteEntity>(query, new
        {
            web_application_id = appId,
            query =  "%" + searchTerm + "%"
        });
    }

    public async Task Update(int id, RouteEntity route)
    {
        const string sql = """

""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            route.Name,
            route.Description
        });
    }

    public async Task<RouteMockResponse> GetMockResponse(int routeId)
    {
        const string query = """

""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<RouteMockResponse>(query, new
        {
            route_id = routeId
        });
    }
}