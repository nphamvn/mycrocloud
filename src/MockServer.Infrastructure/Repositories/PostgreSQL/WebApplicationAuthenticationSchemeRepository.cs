using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using MockServer.Domain.Repositories;
using MockServer.Domain.Settings;
using MockServer.Domain.WebApplication.Entities;
using Microsoft.Extensions.Options;
using MockServer.Domain.WebApplication.Shared;

namespace MockServer.Infrastructure.Repositories.PostgreSql;

public class WebApplicationAuthenticationSchemeRepository : BaseRepository, IWebApplicationAuthenticationSchemeRepository
{
    public WebApplicationAuthenticationSchemeRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public async Task Add(int appId, AuthenticationSchemeEntity auth)
    {
        var query =
"""
INSERT INTO 
    web_application_authentication_scheme(
        web_application_id
        ,type
        ,name
        ,display_name
        ,options
        ,description)
    VALUES (
        @web_application_id
        ,@type
        ,@name
        ,@display_name
        ,@options
        ,@description
    );
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(auth.Type));
        await connection.ExecuteAsync(query, new
        {
            web_application_id = appId,
            type = (int)auth.Type,
            name = auth.Name,
            display_name = auth.DisplayName,
            options = auth.Options,
            description = auth.Description,
        });
    }

    public async Task<IEnumerable<AuthenticationSchemeEntity>> GetAll(int appId)
    {
        var query =
"""
SELECT
    scheme_id SchemeId
    ,web_application_id WebApplicationId
    ,type Type
    ,name Name
    ,description Description
FROM
    web_application_authentication_scheme
WHERE
    web_application_id = @web_application_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<AuthenticationSchemeEntity>(query, new
        {
            web_application_id = appId
        });
    }

    public Task<AuthenticationSchemeEntity> Get(int schemeId, AuthenticationSchemeType type)
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
        SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(type));
        return connection.QuerySingleOrDefaultAsync<AuthenticationSchemeEntity>(query, new
        {
            Id = schemeId
        });
    }

    public async Task Update(int schemeId, AuthenticationSchemeEntity schemeEntity)
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
        SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(schemeEntity.Type));
        int type = (int)schemeEntity.Type;
        await connection.ExecuteAsync(query, new
        {
            Id = schemeId,
            Name = schemeEntity.Name,
            Options = schemeEntity.Options,
            Description = schemeEntity.Description,
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

    public Task<AuthenticationSchemeEntity> Get<TAuthOptions>(int id) where TAuthOptions : AuthenticationSchemeOptions
    {
        Type type = typeof(TAuthOptions);
        if (typeof(JwtBearerAuthenticationOptions).IsEquivalentTo(type))
        {
            SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(AuthenticationSchemeType.JwtBearer));
        }
        else if (typeof(ApiKeyAuthenticationOptions).IsEquivalentTo(type))
        {
            SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(AuthenticationSchemeType.ApiKey));
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
        return connection.QuerySingleOrDefaultAsync<AuthenticationSchemeEntity>(query, new
        {
            Id = id
        });
    }

    public async Task<AuthenticationSchemeEntity> Get(int id)
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
        SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(type));
        return await connection.QuerySingleOrDefaultAsync<AuthenticationSchemeEntity>(query, new
        {
            Id = id
        });
    }
}

public class AuthenticationSchemeOptionsJsonTypeHandler : SqlMapper.TypeHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationSchemeType _type;
    
    public AuthenticationSchemeOptionsJsonTypeHandler(AuthenticationSchemeType type)
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