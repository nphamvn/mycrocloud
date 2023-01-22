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
    public Task Add(int projectId, Authentication auth)
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

    public Task<IEnumerable<Authentication>> GetByProject(int id)
    {
        var query =
                """
                SELECT
                    Id,
                    SchemeName,
                    Type,
                    Description
                FROM
                    ProjectAuthentication
                """;
        using var connection = new SqliteConnection(_connectionString);
        return connection.QueryAsync<Authentication>(query, new
        {
            Id = id
        });
    }

    public Task<Authentication> GetAs(int id, AuthType type)
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
        return connection.QuerySingleOrDefaultAsync<Authentication>(query, new
        {
            Id = id
        });
    }
}

public class AuthenticationOptionsJsonTypeHandler : SqlMapper.TypeHandler<AuthOptions>
{
    private readonly AuthType _type;

    public AuthenticationOptionsJsonTypeHandler(AuthType type)
    {
        _type = type;
    }
    public override AuthOptions Parse(object value)
    {
        var stringValue = value.ToString();
        if (_type is AuthType.JwtBearer)
        {
            return JsonSerializer.Deserialize<JwtBearerAuthenticationOptions>(stringValue);
        }
        else if (_type is AuthType.ApiKey)
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
        if (_type is AuthType.JwtBearer)
        {
            parameter.Value = JsonSerializer.Serialize((JwtBearerAuthenticationOptions)value);
        }
        else if (_type is AuthType.ApiKey)
        {
            parameter.Value = JsonSerializer.Serialize((ApiKeyAuthenticationOptions)value);
        }
        else
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
}