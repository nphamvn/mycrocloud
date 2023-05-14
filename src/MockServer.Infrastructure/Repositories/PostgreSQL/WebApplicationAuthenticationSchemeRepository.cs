using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications.Security;
using MockServer.Core.WebApplications.Security.ApiKey;
using MockServer.Core.WebApplications.Security.JwtBearer;
using Microsoft.Extensions.Options;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class WebApplicationAuthenticationSchemeRepository : BaseRepository, IWebApplicationAuthenticationSchemeRepository
{
    public WebApplicationAuthenticationSchemeRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public Task Add(int appId, AuthenticationScheme auth)
    {
        var query =
                """
                INSERT INTO 
                    WebApplicationAuthenticationScheme (
                        WebApplicationId,
                        Type,
                        Name,
                        Options,
                        Description
                    )
                    VALUES (
                        @WebApplicationId,
                        @Type,
                        @Name,
                        @Options,
                        @Description
                    );
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(auth.Type));
        return connection.ExecuteAsync(query, new
        {
            WebApplicationId = appId,
            Type = (int)auth.Type,
            Name = auth.Name,
            Options = auth.Options,
            Description = auth.Description,
        });
    }

    public Task<IEnumerable<AuthenticationScheme>> GetAll(int appId)
    {
        var query =
                """
                SELECT
                    Id,
                    Name,
                    Type,
                    [Order],
                    Description
                FROM
                    WebApplicationAuthenticationScheme
                WHERE
                    WebApplicationId = @WebApplicationId
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.QueryAsync<AuthenticationScheme>(query, new
        {
            WebApplicationId = appId
        });
    }

    public Task<AuthenticationScheme> Get(int schemeId, AuthenticationSchemeType type)
    {
        var query =
                """
                SELECT
                    Id,
                    Name,
                    Type,
                    [Order],
                    Options,
                    Description
                FROM
                    WebApplicationAuthenticationScheme
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(type));
        return connection.QuerySingleOrDefaultAsync<AuthenticationScheme>(query, new
        {
            Id = schemeId
        });
    }

    public async Task Update(int schemeId, AuthenticationScheme scheme)
    {
        var query =
                """
                UPDATE
                    WebApplicationAuthenticationScheme
                SET
                    Name = @Name,
                    Options = @Options,
                    Description = @Description
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(scheme.Type));
        int type = (int)scheme.Type;
        await connection.ExecuteAsync(query, new
        {
            Id = schemeId,
            Name = scheme.Name,
            Options = scheme.Options,
            Description = scheme.Description,
        });
    }

    public async Task Activate(int appId, List<int> schemeIds)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        var transaction = await connection.BeginTransactionAsync();
        try
        {
            var reset = """
                    UPDATE 
                        WebApplicationAuthenticationScheme 
                    SET 
                        [Order] = null
                    WHERE
                        WebApplicationId = @WebApplicationId;
                    """;
            await connection.ExecuteAsync(reset, new
            {
                WebApplicationId = appId
            }, transaction);

            foreach (var schemeId in schemeIds)
            {
                var set =
                        """
                    UPDATE 
                        WebApplicationAuthenticationScheme 
                    SET 
                        [Order] = (
                            SELECT 
                                (coalesce(MAX([Order]), 0) + 1) 
                            FROM 
                                WebApplicationAuthenticationScheme 
                            WHERE 
                                WebApplicationId = @WebApplicationId) 
                    WHERE
                        Id = @Id
                    """;
                await connection.ExecuteAsync(set, new
                {
                    WebApplicationId = appId,
                    Id = schemeId
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

    public Task<AuthenticationScheme> Get<TAuthOptions>(int id) where TAuthOptions : AuthenticationSchemeOptions
    {
        Type type = typeof(TAuthOptions);
        if (typeof(JwtBearerAuthenticationOptions).IsEquivalentTo(type))
        {
            SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(AuthenticationSchemeType.JwtBearer));
        }
        else if (typeof(ApiKeyAuthenticationOptions).IsEquivalentTo(type))
        {
            SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(AuthenticationSchemeType.ApiKey));
        }
        var query =
                """
                SELECT
                    Id,
                    Name,
                    Type,
                    Options,
                    Description
                FROM
                    WebApplicationAuthenticationScheme
                WHERE
                    Id = @Id
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return connection.QuerySingleOrDefaultAsync<AuthenticationScheme>(query, new
        {
            Id = id
        });
    }

    public async Task<AuthenticationScheme> Get(int id)
    {
        using var connection = new NpgsqlConnection(ConnectionString);
        var type = await connection.QuerySingleOrDefaultAsync<AuthenticationSchemeType>("SELECT Type FROM WebApplicationAuthenticationScheme WHERE Id = @Id", new { Id = id });
        var query =
                """
                SELECT
                    Id,
                    Name,
                    Options,
                    Description
                FROM
                    WebApplicationAuthenticationScheme
                WHERE
                    Id = @Id
                """;
        SqlMapper.AddTypeHandler(new AuthenticationOptionsJsonTypeHandler(type));
        return await connection.QuerySingleOrDefaultAsync<AuthenticationScheme>(query, new
        {
            Id = id
        });
    }
}

public class AuthenticationOptionsJsonTypeHandler : SqlMapper.TypeHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationSchemeType _type;
    
    public AuthenticationOptionsJsonTypeHandler(AuthenticationSchemeType type)
    {
        _type = type;
    }
    public override AuthenticationSchemeOptions Parse(object value)
    {
        var stringValue = value.ToString();
        if (_type is AuthenticationSchemeType.JwtBearer)
        {
            return JsonSerializer.Deserialize<JwtBearerAuthenticationOptions>(stringValue);
        }
        else if (_type is AuthenticationSchemeType.ApiKey)
        {
            return JsonSerializer.Deserialize<ApiKeyAuthenticationOptions>(stringValue);
        }
        else
        {
            return JsonSerializer.Deserialize<AuthenticationSchemeOptions>(stringValue);
        }
    }

    public override void SetValue(IDbDataParameter parameter, AuthenticationSchemeOptions value)
    {
        if (_type is AuthenticationSchemeType.JwtBearer)
        {
            parameter.Value = JsonSerializer.Serialize((JwtBearerAuthenticationOptions)value);
        }
        else if (_type is AuthenticationSchemeType.ApiKey)
        {
            parameter.Value = JsonSerializer.Serialize((ApiKeyAuthenticationOptions)value);
        }
        else
        {
            parameter.Value = JsonSerializer.Serialize(value);
        }
    }
}