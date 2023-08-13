using Dapper;
using Microsoft.Extensions.Options;
using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;
using Npgsql;

namespace WebApp.Infrastructure.Repositories.PostgreSql;
public class WebAppRepository(IOptions<PostgresDatabaseOptions> databaseOptions) : BaseRepository(databaseOptions), IWebAppRepository
{
    public async Task Add(string userId, WebAppEntity app)
    {
        const string sql =
"""
INSERT INTO web_application
(user_id, "name", description)
VALUES(@user_id, @name, @description);
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
DELETE FROM web_application
WHERE app_id = @app_id;
""";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(sql, new
        {
            app_id = appId,
        });
    }

    public async Task<WebAppEntity> FindByUserId(string userId, string name)
    {
        const string sql =
"""
SELECT 
    web_application_id WebAppId,
    user_id, 
    "name", 
    description, 
    created_at, 
    updated_at
FROM
    web_application
WHERE
    user_id = @user_id
    AND "name" = @name
""";
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<WebAppEntity>(sql, new
        {
            user_id = userId,
            name = name
        });
    }

    public async Task<WebAppEntity> Get(int id)
    {
        var query =
                """
                SELECT 
                    web_application_id WebAppId, 
                    user_id UserId, 
                    "name" Name, 
                    description Description,
                    enabled Enabled,
                    created_at CreatedAt, 
                    updated_at UpdatedAt
                FROM
                    web_application wa
                WHERE
                    web_application_id = @web_application_id;
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<WebAppEntity>(query, new
        {
            web_application_id = id
        });
    }

    public async Task<WebAppEntity> FindByUsername(string username, string name)
    {
        var query =
                """
                SELECT 
                    wa.Id,
                    wa.UserId,
                    wa.Name,
                    wa.Description,
                    wa.Accessibility,
                    wa.UseMiddlewares,
                    wa.CreatedAt,
                    wa.UpdatedAt
                FROM
                    WebAppEntity wa
                INNER JOIN 
                    User u ON wa.UserId = u.Id
                WHERE
                    upper(u.Username) = upper(@Username) AND 
                    upper(wa.Name) = upper(@Name);               
                """;   
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return await connection.QuerySingleOrDefaultAsync<WebAppEntity>(query, new
        {
            Username = username,
            Name = name
        });
    }

    public async Task<IEnumerable<WebAppEntity>> Search(string userId, string query, string sort)
    {
        var sql =
                """
                SELECT 
                    web_application_id, 
                    user_id, 
                    "name", 
                    description, 
                    created_at, 
                    updated_at
                FROM
                    web_application
                WHERE
                    user_id = @user_id
                """;
        List<string> conditions = new();
        if (!string.IsNullOrEmpty(query))
        {
            conditions.Add("\"name\" LIKE @query OR description LIKE @query");
        }
        if (conditions.Count > 0)
        {
            sql += " AND " + string.Join(" AND ", conditions);
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
        return await connection.QueryAsync<WebAppEntity>(sql, new
        {
            user_id = userId,
            query = "%" + query + "%"
        });
    }
    public async Task Update(int id, WebAppEntity app)
    {
        var query =
        """
        UPDATE web_application
        SET "name"=@name, description=@description
        WHERE id=@id;
        """;
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(query, new
        {
            id = id,
            name = app.Name,
            description = app.Description
        });
    }
}