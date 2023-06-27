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
            //method = route.Method,
            //path = route.Path,
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
    web_application_id WebApplicationId,
    "name",
    description,
    "authorization"
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
        var sql =
"""
UPDATE
    WebApplicationRoute
SET
    Name = @Name,
    Method = @Method,
    Path = @Path,
    Description = @Description,
    Order = @Order
WHERE
    Id = @Id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            Name = route.Name,
            //Method = route.Method,
            //Path = route.Path,
            Description = route.Description,
            //Order = route.Order,
        });
    }

    public Task<Authorization> GetAuthorization(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<MockResponse> GetMockResponse(int routeId)
    {
        var query =
"""
SELECT
    response_id ResponseId,
    route_id RouteId,
    status_code StatusCode,
    headers Headers,
    body_text BodyText,
    body_text_format BodyTextFormat,
    delay_type DelayType,
    delay_fixed_time DelayFixedTime
FROM
    web_application_route_mock_response
WHERE
    route_id = @route_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<Dictionary<string, string>>());
        return await connection.QuerySingleOrDefaultAsync<MockResponse>(query, new
        {
            route_id = routeId
        });
    }
}