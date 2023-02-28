using System.Text.Json;
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
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RouteRequestQuery>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RouteRequestHeader>>());
        SqlMapper.AddTypeHandler(new JsonTypeHandler<RouteRequestBody>());
        return await connection.QuerySingleOrDefaultAsync<Route>(query, new
        {
            Id = id
        });
    }

    public async Task<IEnumerable<Route>> GetByApplicationId(int appId)
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
                ORDER BY r.Id
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Route>(query, new
        {
            WebApplicationId = appId
        });
    }

    public async Task<RouteRequestBody> GetRequestBody(int requestId)
    {
        var query =
                   """
                    SELECT
                        Required,
                        MatchExactly,
                        Format,
                        Text
                    FROM
                        RequestBody
                    WHERE
                        RequestId = @RequestId
                   """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<RouteRequestBody>(query, new
        {
            RequestId = requestId
        });
    }
    public async Task Update(int id, Route request)
    {
        var query =
                    """
                    UPDATE
                        Requests
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
            Name = request.Name,
            Method = request.Method,
            Path = request.Path,
            Description = request.Description
        });
    }

    public async Task UpdateRequestHeader(int id, IList<RouteRequestHeader> headers)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            const string query =
                            """
                                UPDATE
                                    Requests
                                SET
                                    Headers = @Headers
                                WHERE
                                    Id = @Id
                                """;
            SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RouteRequestHeader>>());
            await connection.ExecuteAsync(query, new
            {
                Headers = headers
            }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdateRequestQuery(int id, IList<RouteRequestQuery> parameters)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var transaction = await connection.BeginTransactionAsync();
        try
        {
            const string query =
                                """
                                UPDATE
                                    Requests
                                SET
                                    Parameters = @Parameters
                                WHERE
                                    Id = @Id
                                """;
            SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RouteRequestQuery>>());
            await connection.ExecuteAsync(query, new
            {
                Parameters = parameters
            }, transaction);
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public Task UpdateRequestBody(int id, RouteRequestBody body)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RouteRequestQuery>> GetRequestQueries(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RouteRequestHeader>> GetRequestHeaders(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Authorization> GetRequestAuthorization(int requestId)
    {
        var query =
                """
                SELECT
                    Authorization
                FROM
                    Requests
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        var json = await connection.QuerySingleOrDefaultAsync<string>(query, new
        {
            Id = requestId
        });
        return !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<Authorization>(json) : null;
    }

    public async Task UpdateRequestAuthorization(int requestId, Authorization authorization)
    {
        var query =
                """
                UPDATE 
                    Requests
                SET
                    Authorization = @Authorization
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = requestId,
            Authorization = JsonSerializer.Serialize(authorization)
        });
    }

    public Task AttachAuthorization(int id, Authorization authorization)
    {
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

    public Task<DirectForwardingIntegration> GetDirectForwardingIntegration(int id)
    {
        throw new NotImplementedException();
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