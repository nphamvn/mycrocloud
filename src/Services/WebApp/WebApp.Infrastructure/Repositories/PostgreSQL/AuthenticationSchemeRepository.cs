using Dapper;
using Npgsql;
using WebApp.Domain.Repositories;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Repositories.PostgreSql;

public class AuthenticationSchemeRepository(IOptions<PostgresDatabaseOptions> databaseOptions) 
    : BaseRepository(databaseOptions), IAuthenticationSchemeRepository
{
    public async Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeEntity scheme)
    {
        const string sql =
"""
insert into 
  jwt_bearer_authentication_scheme (
    app_id, 
    name, 
    display_name, 
    description, 
    issuer, 
    audience, 
  )
values
  (
    @app_id, 
    @name, 
    @display_name, 
    @description, 
    @issuer, 
    @audience
  );
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            app_id = appId,
            name = scheme.Name,
            display_name = scheme.DisplayName,
            description = scheme.Description,
            issuer = scheme.Issuer,
            audience = scheme.Audience
        });
    }

    public async Task<IEnumerable<AuthenticationSchemeEntity>> GetAllByAppId(int appId)
    {
        const string sql =
"""
SELECT
    scheme_id SchemeId,
    app_id AppId,
    1 Type,
    name Name,
    display_name DisplayName,
    description Description,
    created_at CreatedAt,
    updated_at UpdatedAt
FROM
    jwt_bearer_authentication_scheme
WHERE
    app_id = @app_id;
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<AuthenticationSchemeEntity>(sql, new
        {
            app_id = appId,
        });
    }

    public async Task UpdateJwtBearerScheme(int schemeId, JwtBearerAuthenticationSchemeEntity scheme)
    {
        const string sql =
"""
update 
    jwt_bearer_authentication_scheme 
set 
    name = @name,
    display_name = @display_name,
    description = @description,
    issuer = @issuer,
    audience = @audience,
    updated_at = current_timestamp
where 
    scheme_id = @scheme_id;
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            scheme_id = schemeId,
            name = scheme.Name,
            description = scheme.Description,
            issuer = scheme.Issuer,
            audience = scheme.Audience
        });
    }

    public async Task<JwtBearerAuthenticationSchemeEntity> GetJwtBearerScheme(int id)
    {
        const string sql =
"""
SELECT
    scheme_id SchemeId,
    app_id AppId,
    name Name,
    display_name DisplayName,
    description Description,
    issuer Issuer,
    audience Audience,
    created_at CreatedAt,
    updated_at UpdatedAt
FROM
    jwt_bearer_authentication_scheme;
WHERE
    scheme_id = @scheme_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<JwtBearerAuthenticationSchemeEntity>(sql, new
        {
            scheme_id = id
        });
    }
}