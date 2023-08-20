using Dapper;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using Npgsql;

namespace WebApp.Infrastructure.Repositories.PostgreSql;
public class AppRepository(IOptions<PostgresDatabaseOptions> databaseOptions) 
    : BaseRepository(databaseOptions), IAppRepository
{
    public async Task Add(string userId, AppEntity app)
    {
        const string sql =
"""
insert into 
  app (
    user_id, 
    name, 
    description
  )
values
  (
    $user_id, 
    $name, 
    $description, 
  );
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            user_id = userId,
            name = app.Name,
            description = app.Description
        });
    }

    public async Task Delete(int appId)
    {
        const string sql =
"""
  delete from 
    app 
  where 
    app_id = @app_id;
""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            app_id = appId,
        });
    }

    public async Task<AppEntity> FindByUserIdAndAppName(string userId, string name)
    {
        const string sql =
"""
SELECT
    app_id AppId,
    user_id UserId,
    name Name,
    description Description,
    created_at CreatedAt,
    updated_at UpdatedAt
FROM
    app
WHERE
    user_id = @user_id
    AND "name" = @name
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<AppEntity>(sql, new
        {
            user_id = userId,
            name = name
        });
    }

    public async Task<AppEntity> GetByAppId(int id)
    {
        const string sql =
"""
SELECT
    app_id AppId,
    user_id UserId,
    name Name,
    description Description,
    created_at CreatedAt,
    updated_at UpdatedAt
FROM
    app
WHERE
    app_id = @app_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<AppEntity>(sql, new
        {
            app_id = id
        });
    }

    public async Task<IEnumerable<AppEntity>> ListByUserId(string userId, string query, string sort)
    {
        var sql =
"""
SELECT
    app_id AppId,
    user_id UserId,
    name Name,
    description Description,
    created_at CreatedAt,
    updated_at UpdatedAt
FROM
    app
WHERE
    user_id = @user_id
""";
        List<string> conditions = new();
        if (!string.IsNullOrEmpty(query))
        {
            conditions.Add("name LIKE @query OR description LIKE @query");
        }
        if (conditions.Count != 0)
        {
            conditions.Add(" ");
            sql += string.Join(" AND ", conditions);
        }
        if (!string.IsNullOrEmpty(sort))
        {
            sql += " ORDER BY " + sort;
        }
        else
        {
            sql += " ORDER BY updated_at DESC";
        }

        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QueryAsync<AppEntity>(sql, new
        {
            user_id = userId,
            query = "%" + query + "%"
        });
    }
    public async Task Update(int appId, AppEntity app)
    {
        const string sql =
"""
update 
  app 
set 
  name = @name,
  description = @description,
  updated_at = current_timestamp
where 
    app_id = @app_id
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            app_id = appId,
            name = app.Name,
            description = app.Description
        });
    }
}