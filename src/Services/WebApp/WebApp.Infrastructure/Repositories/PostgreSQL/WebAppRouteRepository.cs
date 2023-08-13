using Dapper;
using Microsoft.Extensions.Options;
using Npgsql;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class WebAppRouteRepository(IOptions<PostgresDatabaseOptions> databaseOptions) 
    : BaseRepository(databaseOptions), IWebAppRouteRepository
{
    public async Task<int> Add(int appId, RouteEntity route)
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

    public async Task<RouteEntity> GetById(int id)
    {
        const string query = """
SELECT
	war.route_id RouteId,
	war.web_application_id WebAppId,
	war."name" Name,
    war.description Description,
	"path" MatchPath,
    COALESCE(JSON_AGG(rmmb.method) FILTER (WHERE rmmb.method IS NOT NULL), '[]') MatchMethods,
    war.match_order MatchOrder,
    war.authorization_type AuthorizationType,
    war.integration_type ResponseProvider,
	war.created_date CreatedDate,
    war.updated_date UpdatedDate
FROM
	public.web_application_route war
	INNER JOIN web_application wa ON wa.web_application_id = war.web_application_id
	LEFT JOIN  web_app_route_match_method_bind rmmb ON rmmb.route_id = war.route_id
WHERE
	war.route_id  = @route_id
GROUP BY
	war.route_id
""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<MatchMethodCollection>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<RouteAuthorization>());
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
            query = "%" + searchTerm + "%"
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

    public async Task<IEnumerable<RouteEntity>> List(int appId, string searchTerm, string sort)
    {
        var query =
"""
SELECT
	war.route_id RouteId,
	war.web_application_id WebAppId,
	war."name" Name,
    war.description Description,
	"path" MatchPath,
    COALESCE(JSON_AGG(rmmb.method) FILTER (WHERE rmmb.method IS NOT NULL), '[]') MatchMethods,
    war.match_order MatchOrder,
    war.authorization_type AuthorizationType,
    war.integration_type ResponseProvider,
	war.created_date CreatedDate,
    war.updated_date UpdatedDate
FROM
	web_application_route war
INNER JOIN
	web_application wa ON wa.web_application_id = war.web_application_id
    LEFT JOIN web_app_route_match_method_bind rmmb ON rmmb.route_id = war.route_id
WHERE 
	wa.web_application_id = @web_application_id
GROUP BY
    war.route_id
/**/
""";
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query +=
                    """
                    AND war."name" LIKE @query OR "path" LIKE @query
                    """;
        }
        if (!string.IsNullOrEmpty(sort))
        {
            query +=
                """
                /**/
                ORDER BY route_id
                """;
        }
        SqlMapper.AddTypeHandler(new JsonTypeHandler<MatchMethodCollection>());
        await using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<RouteEntity>(query, new
        {
            web_application_id = appId,
            query = "%" + searchTerm + "%"
        });
    }
}