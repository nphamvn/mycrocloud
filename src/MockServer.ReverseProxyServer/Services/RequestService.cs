using MockServer.ReverseProxyServer.Interfaces;
using MockServer.ReverseProxyServer.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using MockServer.ReverseProxyServer.Entities;
using Microsoft.Data.Sqlite;

namespace MockServer.ReverseProxyServer.Services;

public class RequestServices : IRequestServices
{
    private readonly string _connectionString;

    public RequestServices(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SQLite");
    }

    public async Task<AppRequest> GetBaseRequest(RequestModel model)
    {
        var sql =
                """
                SELECT *
                    FROM Requests r
                        INNER JOIN
                        Users u ON r.UserId = u.Id
                    WHERE u.Username = "nampham" AND 
                          r.Method = "GET" AND 
                         r.Path = "foo";
                """;

        using var connection = new SqliteConnection(_connectionString);
        var request = await connection.QuerySingleAsync<Request>(sql);

        return new AppRequest();
    }

    public async Task<AppRequest> GetRequest(RequestModel model)
    {
        AppRequest appRequest = null;

        if ("type" == "fixed")
        {
            appRequest = new FixedRequest();
        }
        return appRequest;
    }
}