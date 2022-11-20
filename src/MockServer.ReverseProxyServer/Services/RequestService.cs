using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities;
using AutoMapper;

namespace MockServer.ReverseProxyServer.Services;

public class RequestServices : IRequestServices
{
    private readonly string _connectionString;
    private readonly IMapper _mapper;
    public RequestServices(IConfiguration configuration, IMapper mapper)
    {
        _connectionString = configuration.GetConnectionString("SQLite");
        _mapper = mapper;
    }

    public async Task<AppRequest> FindRequest(IncomingRequest model)
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
        var request = await connection.QuerySingleAsync<Request>(sql, new
        {
            username = model.Username,
            projectName = model.ProjectName,
            method = model.Method,
            path = model.Path
        });
        return _mapper.Map<AppRequest>(request);
    }

    public async Task<AppRequest> GetRequest(IncomingRequest model)
    {
        AppRequest appRequest = null;

        if ("type" == "fixed")
        {
            appRequest = new MockServer.ReverseProxyServer.Models.FixedRequest();
        }
        return appRequest;
    }
}