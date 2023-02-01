using MockServer.Api.Interfaces;
using MockServer.Api.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Models.Requests;
using AutoMapper;

namespace MockServer.Api.Services;

public class RequestServices : IRequestServices
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;
    public RequestServices(IConfiguration configuration, IMapper mapper)
    {
        _connectionString = configuration.GetConnectionString("SQLite");
        _mapper = mapper;
    }

    public async Task<Models.Request> FindRequest(IncomingRequest model)
    {
        var sql =
                """
                SELECT *
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
        var request = await connection.QuerySingleAsync<Core.Models.Requests.Request>(sql, new
        {
            username = model.Username,
            projectName = model.ProjectName,
            method = model.Method,
            path = model.Path
        });

        return _mapper.Map<Models.Request>(request);
    }

    public async Task<Models.Request> GetRequest(IncomingRequest model)
    {
        Models.Request appRequest = null;

        if ("type" == "fixed")
        {
            appRequest = new MockServer.Api.Models.FixedRequest();
        }
        return appRequest;
    }
}