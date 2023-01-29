using System.Data;
using System.Text.Json;
using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly string _connectionString;
    public AuthRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }
    public Task Add(int projectId, AppAuthentication auth)
    {
        var query =
                """
                INSERT INTO 
                    ProjectAuthentication (
                        ProjectId,
                        Type,
                        SchemeName,
                        Options,
                        Description
                    )
                    VALUES (
                        @ProjectId,
                        @Type,
                        @SchemeName,
                        @Options,
                        @Description
                    );
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(auth.Type));
        int type = (int)auth.Type;
        return connection.ExecuteAsync(query, new
        {
            ProjectId = projectId,
            Type = type,
            SchemeName = auth.SchemeName,
            Options = auth.Options,
            Description = auth.Description,
        });
    }

    public Task<IEnumerable<AppAuthentication>> GetByProject(int id)
    {
        var query =
                """
                SELECT
                    Id,
                    SchemeName,
                    Type,
                    [Order],
                    Description
                FROM
                    ProjectAuthentication
                WHERE
                    ProjectId = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryAsync<AppAuthentication>(query, new
        {
            Id = id
        });
    }

    public Task<AppAuthentication> GetAs(int id, AuthenticationType type)
    {
        var query =
                """
                SELECT
                    Id,
                    SchemeName,
                    Options,
                    Description
                FROM
                    ProjectAuthentication
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(type));
        return connection.QuerySingleOrDefaultAsync<AppAuthentication>(query, new
        {
            Id = id
        });
    }

    public async Task<AppAuthorization> GetRequestAuthorization(int requestId)
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
        return !string.IsNullOrEmpty(json) ? JsonSerializer.Deserialize<AppAuthorization>(json) : null;
    }

    public async Task SetRequestAuthorization(int requestId, AppAuthorization authorization)
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

    public async Task Update(int id, AppAuthentication auth)
    {
        var query =
                """
                UPDATE
                    ProjectAuthentication
                SET
                    SchemeName = @SchemeName,
                    Options = @Options,
                    Description = @Description
                WHERE
                    Id = @Id
                """;
        using var connection = new SqliteConnection(_connectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(auth.Type));
        int type = (int)auth.Type;
        await connection.ExecuteAsync(query, new
        {
            Id = id,
            SchemeName = auth.SchemeName,
            Options = auth.Options,
            Description = auth.Description,
        });
    }

    public async Task SetProjectAuthentication(int projectId, List<int> schemeIds)
    {
        using var connection = new SqliteConnection(_connectionString);

        var reset = """
                    UPDATE ProjectAuthentication SET [Order] = null WHERE ProjectId = @ProjectId;
                    """;
        await connection.ExecuteAsync(reset, new
        {
            ProjectId = projectId
        });
        foreach (var id in schemeIds)
        {
            var set =
                    """
                    UPDATE 
                        ProjectAuthentication 
                    SET 
                        [Order] = (
                            SELECT 
                                (coalesce(MAX([Order]), 0) + 1) 
                            FROM 
                                ProjectAuthentication 
                            WHERE 
                                ProjectId = @ProjectId) 
                    WHERE Id = @Id
                    """;
            await connection.ExecuteAsync(set, new
            {
                ProjectId = projectId,
                Id = id
            });
        }
    }
}

public class AuthenticationOptionsJsonTypeHandler : SqlMapper.TypeHandler<AuthOptions>
{
    private readonly AuthenticationType _type;

    public AuthenticationOptionsJsonTypeHandler(AuthenticationType type)
    {
        _type = type;
    }
    public override AuthOptions Parse(object value)
    {
        var stringValue = value.ToString();
        if (_type is AuthenticationType.JwtBearer)
        {
            return JsonSerializer.Deserialize<JwtBearerAuthenticationOptions>(stringValue);
        }
        else if (_type is AuthenticationType.ApiKey)
        {
            return JsonSerializer.Deserialize<ApiKeyAuthenticationOptions>(stringValue);
        }
        else
        {
            return JsonSerializer.Deserialize<AuthOptions>(stringValue);
        }
    }

    public override void SetValue(IDbDataParameter parameter, AuthOptions value)
    {
        if (_type is AuthenticationType.JwtBearer)
        {
            parameter.Value = JsonSerializer.Serialize((JwtBearerAuthenticationOptions)value);
        }
        else if (_type is AuthenticationType.ApiKey)
        {
            parameter.Value = JsonSerializer.Serialize((ApiKeyAuthenticationOptions)value);
        }
        else
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
}