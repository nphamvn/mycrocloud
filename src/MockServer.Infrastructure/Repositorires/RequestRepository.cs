using System.Collections.Generic;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities.Requests;
using MockServer.Core.Enums;
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
            Method = (int)request.Method,
            Path = request.Path,
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
            var delete1 = "DELETE FROM CallbackRequest WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete1, new { Id = id }, trans);

            var delete2 = "DELETE FROM FixedRequestBody WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete2, new { Id = id }, trans);

            var delete3 = "DELETE FROM FixedRequestParams WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete3, new { Id = id }, trans);

            var delete4 = "DELETE FROM FixedRequestResponse WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete4, new { Id = id }, trans);

            var delete5 = "DELETE FROM FixedResponseHeader WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete5, new { Id = id }, trans);

            var delete6 = "DELETE FROM ForwardingRequest WHERE RequestId = @Id";
            await connection.ExecuteAsync(delete6, new { Id = id }, trans);

            var query = "DELETE FROM Requests WHERE Id = @Id;";
            await connection.ExecuteAsync(query, new { Id = id }, trans);

            await trans.CommitAsync();
        }
        catch (Exception)
        {
            await trans.RollbackAsync();
            throw;
        }
    }

    public async Task<Request> FindRequest(string username, string projectName, string method, string path)
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

    public async Task<Request> FindRequest(int userId, string projectName, RequestMethod method, string path)
    {
        var query =
                """
                SELECT
                    r.Id,
                    r.Type,
                    r.Name,
                    r.Path,
                    r.Description,
                    m.Id,
                    r.ProjectId
                FROM Requests r
                    INNER JOIN
                    Project p ON r.ProjectId = p.Id
                    INNER JOIN
                    Users u ON p.UserId = u.Id
                    INNER JOIN
                    Master_HttpMethod m ON r.Method = m.Id
                WHERE u.Id = @userId AND 
                    upper(p.Name) = upper(@projectName) AND 
                    m.Id = @method AND 
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

    public async Task<FixedResponse> GetFixedResponse(int requestId)
    {
        var query =
                """
                SELECT res.*
                FROM FixedRequestResponse res
                    INNER JOIN
                    Requests req ON res.RequestId = req.Id
                WHERE req.Id = @id;
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<FixedResponse>(query, new
        {
            id = requestId
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
                        Description
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

    public async Task SaveFixedRequestConfig(int userId, string projectName, int requestId, FixedRequest config)
    {
        var query =
                """
                INSERT INTO 
                    FixedRequestResponse 
                    (
                        RequestId,
                        StatusCode,
                        ContentType,
                        Body,
                        Delay
                     )
                     VALUES (
                         @RequestId,
                         @StatusCode,
                         @ContentType,
                         @Body,
                         @Delay
                     );
                """;

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            RequestId = requestId,
            StatusCode = config.ResponseStatusCode,
            ContentType = config.ResponseContentType,
            Body = config.ResponseBody,
            Delay = config.Delay
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
                    Required = p.Required,
                    MatchExactly = p.MatchExactly,
                    Description = p.Description
                })
                .ToList();
                var insertQuery =
                        """
                         INSERT INTO RequestParams (
                            RequestId,
                            Key,
                            Value,
                            Required,
                            MatchExactly,
                            Description
                        ) VALUES(
                            @RequestId,
                            @Key,
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
}