using System.Data;
using System.Text.Json;
using Dapper;
using Npgsql;
using WebApp.Domain.Repositories;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Domain.Shared;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class WebAppAuthenticationSchemeRepository(IOptions<PostgresDatabaseOptions> databaseOptions) : BaseRepository(databaseOptions), IWebAppAuthenticationSchemeRepository
{
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

    public async Task<IEnumerable<AuthenticationSchemeEntity>> GetAll(string userId, string appName)
    {
        var query =
"""
select
	scheme_id SchemeId,
	waas.web_application_id WebAppId,
	"type" Type,
	waas."name" Name,
	waas.description Description,
	display_name DisplayName
from
	public.web_application_authentication_scheme waas
	inner join web_application wa on wa.web_application_id = waas.web_application_id
where 
	wa.user_id = @user_id and wa."name" = @app_name;
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<AuthenticationSchemeEntity>(query, new
        {
            user_id = userId,
            app_name = appName
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

    public async Task Update(int schemeId, AuthenticationSchemeEntity scheme)
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
        SqlMapper.AddTypeHandler(new AuthenticationSchemeOptionsJsonTypeHandler(scheme.Type));
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
                        WebAppId = @WebAppId;
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
                                WebAppId = @WebAppId) 
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

public class AuthenticationSchemeOptionsJsonTypeHandler(AuthenticationSchemeType type) : SqlMapper.TypeHandler<AuthenticationSchemeOptions>
{
    private readonly AuthenticationSchemeType _type = type;

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