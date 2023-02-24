using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Models.Requests;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
namespace MockServer.Infrastructure.Repositories;

public class RequestRepository : IRequestRepository
{
    private readonly string _connectionString;
    public RequestRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }

    public async Task<int> Create(int projectId, Request request)
    {
        var query =
                """
                INSERT INTO 
                    Requests 
                    (
                        ProjectId,
                        Type,
                        Name,
                        Method,
                        Path,
                        Description
                     )
                     VALUES (
                        @ProjectId,
                        @Type,
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
            ProjectId = projectId,
            Type = (int)request.Type,
            Name = request.Name,
            Method = request.Method,
            Path = request.Path,
            Authorization = request.Authorization,
            Description = request.Description
        });
    }

    public async Task Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var trans = await connection.BeginTransactionAsync();
        try
        {
            var delete = "DELETE FROM Requests WHERE Id = @Id;";
            await connection.ExecuteAsync(delete, new { Id = id }, trans);

            await trans.CommitAsync();
        }
        catch (Exception)
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<Request> Find(int projectId, string method, string path)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.Type,
                    r.Name,
                    r.Path,
                    r.Method,
                    r.Description
                FROM Requests r
                    INNER JOIN
                        Project p ON r.ProjectId = p.Id AND r.ProjectId = @ProjectId
                WHERE
                    upper(r.method) = upper(@method) AND 
                    upper(r.Path) = upper(@path);
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            ProjectId = projectId,
            method = method,
            path = path
        });
    }

    public async Task<Request> GetById(int id)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.ProjectId,
                    r.Type,
                    r.Name,
                    r.Path,
                    r.Method,
                    r.Description
                FROM Requests r
                WHERE r.Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            Id = id
        });
    }

    public async Task<ForwardingRequest> GetForwardingRequest(int requestId)
    {
        var query =
                """
                SELECT fr.Id,
                    fr.RequestId,
                    fr.Scheme,
                    fr.Host
                FROM ForwardingRequest fr
                    INNER JOIN
                    Requests r ON fr.RequestId = r.Id
                WHERE r.Id = @id;
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<ForwardingRequest>(query, new
        {
            id = requestId
        });
    }

    public async Task<IEnumerable<Request>> GetByProjectId(int ProjectId)
    {
        var query =
                """
                SELECT 
                    r.Id,
                    r.Type,
                    r.Name,
                    r.Method,
                    r.Path
                FROM Requests r
                    INNER JOIN
                        Project p ON r.ProjectId = p.Id
                WHERE
                    p.Id = @ProjectId;
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Request>(query, new
        {
            ProjectId = ProjectId
        });
    }

    public async Task<RequestBody> GetRequestBody(int requestId)
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
        return await connection.QuerySingleOrDefaultAsync<RequestBody>(query, new
        {
            RequestId = requestId
        });
    }

    public async Task<Response> GetResponse(int requestId)
    {
        var query =
                   """
                    SELECT
                        StatusCode,
                        BodyText,
                        BodyTextRenderEngine,
                        BodyRenderScript,
                        Delay,
                        DelayTime
                    FROM
                        Response
                    WHERE
                        RequestId = @RequestId
                   """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Response>(query, new
        {
            RequestId = requestId
        });
    }

    public async Task<IEnumerable<ResponseHeader>> GetResponseHeaders(int id)
    {
        var query =
                      """
                    SELECT
                        Id,
                        Name,
                        Value
                    FROM
                        ResponseHeaders
                    WHERE
                        RequestId = @RequestId
                   """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<ResponseHeader>(query, new
        {
            RequestId = id
        });
    }

    public async Task Update(int id, Request request)
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

    

    public async Task UpdateRequestHeader(int id, IList<RequestHeader> headers)
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
            SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestHeader>>());
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

    public async Task UpdateRequestQuery(int id, IList<RequestQuery> parameters)
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
            SqlMapper.AddTypeHandler(new JsonTypeHandler<IList<RequestQuery>>());
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

    public async Task UpdateResponse(int id, FixedRequest config)
    {
        if (config.Response is Response response)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var exists = await connection.ExecuteScalarAsync<bool>("SELECT COUNT(1) FROM Response WHERE RequestId = @RequestId",
                new
                {
                    RequestId = id
                });
                if (!exists)
                {
                    var insert =
                        """
                         INSERT INTO Response (
                            RequestId,
                            StatusCode,
                            BodyTextRenderEngine,
                            BodyText,
                            BodyRenderScript,
                            Delay,
                            DelayTime
                        ) VALUES(
                            @RequestId,
                            @StatusCode,
                            @BodyTextRenderEngine,
                            @BodyText,
                            @BodyRenderScript,
                            @Delay,
                            @DelayTime
                        )
                        """;
                    await connection.ExecuteAsync(insert, new
                    {
                        RequestId = id,
                        StatusCode = response.StatusCode,
                        BodyText = response.BodyText,
                        BodyTextRenderEngine = response.BodyTextRenderEngine,
                        BodyRenderScript = response.BodyRenderScript,
                        Delay = response.Delay,
                        DelayTime = response.DelayTime
                    }, transaction);
                }
                else
                {
                    var update =
                        """
                        UPDATE
                            Response
                        SET
                            StatusCode = @StatusCode,
                            BodyTextRenderEngine = @BodyTextRenderEngine,
                            BodyText = @BodyText,
                            BodyRenderScript = @BodyRenderScript,
                            Delay = @Delay,
                            DelayTime = @DelayTime
                        WHERE
                            RequestId = @RequestId
                        """;
                    await connection.ExecuteAsync(update, new
                    {
                        RequestId = id,
                        StatusCode = response.StatusCode,
                        BodyText = response.BodyText,
                        BodyTextRenderEngine = response.BodyTextRenderEngine,
                        BodyRenderScript = response.BodyRenderScript,
                        Delay = response.Delay,
                        DelayTime = response.DelayTime
                    }, transaction);
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task UpdateResponseHeaders(int id, FixedRequest config)
    {
        if (config.ResponseHeaders is IList<ResponseHeader> headers)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var deleteQuery = "DELETE FROM ResponseHeaders WHERE RequestId = @RequestId";
                await connection.ExecuteAsync(deleteQuery, new
                {
                    RequestId = id,
                }, transaction);

                var map = headers.Select(h => new
                {
                    RequestId = id,
                    Name = h.Name,
                    Value = h.Value
                })
                .ToList();
                var insertQuery =
                        """
                         INSERT INTO ResponseHeaders (
                            RequestId,
                            Name,
                            Value
                        ) VALUES(
                            @RequestId,
                            @Name,
                            @Value
                        )
                        """;

                await connection.ExecuteAsync(insertQuery, map, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public Task UpdateRequestBody(int id, RequestBody body)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RequestQuery>> GetRequestQueries(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<RequestHeader>> GetRequestHeaders(int id)
    {
        throw new NotImplementedException();
    }

    public Task AttachAuthorization(int id, Authorization authorization)
    {
        throw new NotImplementedException();
    }

    public Task<Authorization> GetAuthorization(int id)
    {
        throw new NotImplementedException();
    }
}
public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override void SetValue(System.Data.IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }

    public override T Parse(object value)
    {
        if (value is string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        return default(T);
    }
}