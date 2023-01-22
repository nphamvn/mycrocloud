using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities.Requests;
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
        SqlMapper.AddTypeHandler(new AuthorizationJsonTypeHandler());
    }

    public async Task<int> Create(int userId, string projectName, Request request)
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
                        Authorization,
                        Description
                     )
                     VALUES (
                        (SELECT Id
                          FROM Project
                          WHERE UserId = @UserId AND 
                            Name = @ProjectName),
                        @Type,
                        @Name,
                        @Method,
                        @Path,
                        @Authorization,
                        @Description
                     );
                    SELECT last_insert_rowid();
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleAsync<int>(query, new
        {
            UserId = userId,
            ProjectName = projectName,
            Type = (int)request.Type,
            Name = request.Name,
            Method = request.Method,
            Path = request.Path,
            Authorization = request.Authorization,
            Description = request.Description
        });
    }

    public async Task Delete(int userId, string projectName, int id)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();
        using var trans = await connection.BeginTransactionAsync();
        try
        {
            var delete1 = "DELETE FROM CallbackRequest WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete1, new { RequestId = id }, trans);

            var delete2 = "DELETE FROM FixedRequestConfiguration WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete2, new { RequestId = id }, trans);

            var delete3 = "DELETE FROM ForwardingRequest WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete3, new { RequestId = id }, trans);

            var delete4 = "DELETE FROM RequestBody WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete4, new { RequestId = id }, trans);

            var delete5 = "DELETE FROM RequestHeaders WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete5, new { RequestId = id }, trans);

            var delete6 = "DELETE FROM RequestParams WHERE RequestId = @RequestId";
            await connection.ExecuteAsync(delete6, new { RequestId = id }, trans);

            var query = "DELETE FROM Response WHERE RequestId = @RequestId;";
            await connection.ExecuteAsync(query, new { RequestId = id }, trans);

            var delete7 = "DELETE FROM ResponseHeaders WHERE RequestId = @RequestId;";
            await connection.ExecuteAsync(delete7, new { RequestId = id }, trans);

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

    public async Task<Request> Get(string username, string projectName, string method, string path)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.Type,
                    r.Name,
                    r.Path,
                    r.Description,
                    m.UpperCaseName AS Method,
                    r.ProjectId
                FROM Requests r
                    INNER JOIN
                    Project p ON r.ProjectId = p.Id
                    INNER JOIN
                    Users u ON p.UserId = u.Id
                    INNER JOIN
                    Master_HttpMethod m ON r.Method = m.Id
                WHERE upper(u.Username) = upper(@username) AND 
                    upper(p.Name) = upper(@projectName) AND 
                    m.UpperCaseName = upper(@method) AND 
                    upper(r.Path) = upper(@path);
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            username = username,
            projectName = projectName,
            method = method,
            path = path
        });
    }

    public async Task<Request> Get(int userId, string projectName, string method, string path)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.Type,
                    r.Name,
                    r.Path,
                    r.Method,
                    r.Description,
                    r.ProjectId
                FROM Requests r
                    INNER JOIN
                    Project p ON r.ProjectId = p.Id
                    INNER JOIN
                    Users u ON p.UserId = u.Id
                WHERE u.Id = @userId AND 
                    upper(p.Name) = upper(@projectName) AND 
                    upper(r.method) = upper(@method) AND 
                    upper(r.Path) = upper(@path);
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            userId = userId,
            projectName = projectName,
            method = method,
            path = path
        });
    }

    public async Task<Request> Get(int userId, string projectName, int id)
    {
        var query =
                """
                SELECT r.*
                FROM Requests r
                    INNER JOIN
                    Project p ON r.ProjectId = p.Id
                    INNER JOIN
                    Users u ON p.UserId = u.Id
                WHERE r.Id = @RequestId AND 
                    u.Id = @UserId AND 
                    lower(p.Name) = lower(@ProjectName);
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            RequestId = id,
            UserId = userId,
            ProjectName = projectName
        });
    }

    public async Task<Request> Get(int projectId, string method, string path)
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
                WHERE r.ProjectId = @ProjectId AND 
                    upper(r.Method) = upper(@Method) AND 
                    upper(r.Path) = upper(@Path);
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            ProjectId = projectId,
            Method = method,
            Path = path
        });
    }

    public async Task<Request> Get(int id)
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
                WHERE r.Id = @Id
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Request>(query, new
        {
            Id = id
        });
    }

    public async Task<FixedRequest> GetFixedRequestConfig(int userId, string projectName, int id)
    {
        var query =
                """
                SELECT res.RequestId,
                    res.StatusCode ResponseStatusCode,
                    res.Body ResponseBody
                FROM FixedRequestResponse res
                    INNER JOIN
                    Requests req ON res.RequestId = req.Id
                WHERE req.Id = @Id;
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<FixedRequest>(query, new
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

    public async Task<IEnumerable<Request>> GetProjectRequests(int ProjectId)
    {
        var query =
                """
                SELECT r.Id,
                    r.ProjectId,
                    r.Type,
                    r.Name,
                    r.Method,
                    r.Path
                FROM Requests r
                    INNER JOIN
                    Project p ON r.ProjectId = p.Id
                WHERE p.Id = @ProjectId;
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

    public async Task<IEnumerable<RequestHeader>> GetRequestHeaders(int id)
    {
        var query =
                      """
                    SELECT
                        Id,
                        Name,
                        Value,
                        Required,
                        MatchExactly,
                        Description
                    FROM
                        RequestHeaders
                    WHERE
                        RequestId = @RequestId
                   """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<RequestHeader>(query, new
        {
            RequestId = id
        });
    }

    public async Task<IEnumerable<RequestParam>> GetRequestParams(int id)
    {
        var query =
                   """
                    SELECT
                        Id,
                        Key,
                        Value,
                        Required,
                        MatchExactly,
                        Description,
                        Constraints
                    FROM
                        RequestParams
                    WHERE
                        RequestId = @RequestId
                   """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<RequestParam>(query, new
        {
            RequestId = id
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

    public async Task Update(int userId, string projectName, int id, Request request)
    {
        var query =
                    """
                    UPDATE
                        Requests
                    SET
                        Type = @Type,
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
            Type = (int)request.Type,
            Name = request.Name,
            Method = request.Method,
            Path = request.Path,
            Description = request.Description
        });
    }

    public async Task UpdateRequestBody(int id, FixedRequest config)
    {
        if (config.RequestBody is RequestBody body)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var deleteQuery = "DELETE FROM RequestBody WHERE RequestId = @RequestId";
                await connection.ExecuteAsync(deleteQuery, new
                {
                    RequestId = id,
                }, transaction);
                var insertQuery =
                        """
                         INSERT INTO RequestBody (
                            RequestId,
                            Required,
                            MatchExactly,
                            Format,
                            Text
                        ) VALUES(
                            @RequestId,
                            @Required,
                            @MatchExactly,
                            @Format,
                            @Text
                        )
                        """;

                await connection.ExecuteAsync(insertQuery, new
                {
                    RequestId = id,
                    Required = body.Required,
                    MatchExactly = body.MatchExactly,
                    Format = body.Format,
                    Text = body.Text
                }, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task UpdateRequestHeaders(int id, FixedRequest config)
    {
        if (config.RequestHeaders is IList<RequestHeader> headers)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var deleteQuery = "DELETE FROM RequestHeaders WHERE RequestId = @RequestId";
                await connection.ExecuteAsync(deleteQuery, new
                {
                    RequestId = id,
                }, transaction);

                var map = headers.Select(h => new
                {
                    RequestId = id,
                    Name = h.Name,
                    Value = h.Value,
                    Required = h.Required,
                    MatchExactly = h.MatchExactly,
                    Description = h.Description
                })
                .ToList();
                var insertQuery =
                        """
                         INSERT INTO RequestHeaders (
                            RequestId,
                            Name,
                            Value,
                            Required,
                            MatchExactly,
                            Description
                        ) VALUES(
                            @RequestId,
                            @Name,
                            @Value,
                            @Required,
                            @MatchExactly,
                            @Description
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

    public async Task UpdateRequestParams(int id, FixedRequest config)
    {
        if (config.RequestParams is IList<RequestParam> @params)
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();
            try
            {
                var deleteQuery = "DELETE FROM RequestParams WHERE RequestId = @RequestId";
                await connection.ExecuteAsync(deleteQuery, new
                {
                    RequestId = id,
                }, transaction);

                var map = @params.Select(p => new
                {
                    RequestId = id,
                    Key = p.Key,
                    Value = p.Value,
                    MatchExactly = p.MatchExactly,
                    Description = p.Description,
                    Constraints = p.Constraints
                })
                .ToList();
                var insertQuery =
                        """
                         INSERT INTO RequestParams (
                            RequestId,
                            Key,
                            Value,
                            Description,
                            Constraints
                        ) VALUES(
                            @RequestId,
                            @Key,
                            @Value,
                            @Description,
                            @Constraints
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
}

public class AuthorizationJsonTypeHandler : SqlMapper.TypeHandler<AppAuthorization>
{
    public override AppAuthorization Parse(object value)
    {
        return JsonSerializer.Deserialize<AppAuthorization>(value.ToString());
    }

    public override void SetValue(System.Data.IDbDataParameter parameter, AppAuthorization value)
    {
        parameter.Value = JsonSerializer.Serialize((AppAuthorization)value);
    }
}