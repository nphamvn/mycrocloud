using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities;
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

    public async Task Create(int userId, string projectName, Request request)
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
                         Path
                     )
                     VALUES (
                         (SELECT Id
                          FROM Project
                          WHERE UserId = @UserId AND 
                            Name = @ProjectName),
                         @Type,
                         @Name,
                         @Method,
                         @Path
                     );
                """;

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            UserId = userId,
            ProjectName = projectName,
            Type = (int)request.Type,
            Name = request.Name,
            Method = (int)request.Method,
            Path = request.Path
        });
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
}