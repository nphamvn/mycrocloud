using Dapper;
using Microsoft.Extensions.Options;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications;
using Npgsql;

namespace MockServer.Infrastructure.Repositories.PostgreSql;
public class WebApplicationRepository : BaseRepository, IWebApplicationRepository
{
    public WebApplicationRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public async Task Add(string userId, WebApplication app)
    {
        var query =
        """
        INSERT INTO web_application
        (user_id, "name", description)
        VALUES(@user_id, @name, @description);
        """;
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(query, new
        {
            user_id = userId,
            name = app.Name,
            description = app.Description
        });
    }

    public async Task Delete(int id)
    {
        var query =
        """
        DELETE FROM web_application
        WHERE id=@id;        
        """;
        using var connection = new NpgsqlConnection(ConnectionString);
        await connection.ExecuteAsync(query, new
        {
            id = id
        });
    }

    public async Task<WebApplication> FindByUserId(string userId, string name)
    {
        var query =
                """
                SELECT 
                    id, 
                    user_id, 
                    "name", 
                    description, 
                    created_at, 
                    updated_at
                FROM
                    web_application
                WHERE
                    user_id = @user_id
                    AND "name" = @name;
                    ;             
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<WebApplication>(query, new
        {
            user_id = userId,
            name = name
        });
    }

    public async Task<WebApplication> Get(int id)
    {
        var query =
                """
                SELECT 
                    id, 
                    user_id, 
                    "name", 
                    description, 
                    created_at, 
                    updated_at
                FROM
                    web_application
                WHERE
                    id = @id;
                """;
        using var connection = new NpgsqlConnection(ConnectionString);
        return await connection.QuerySingleOrDefaultAsync<WebApplication>(query, new
        {
            id = id
        });
    }

    public async Task<WebApplication> FindByUsername(string username, string name)
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
                    WebApplication wa
                INNER JOIN 
                    User u ON wa.UserId = u.Id
                WHERE
                    upper(u.Username) = upper(@Username) AND 
                    upper(wa.Name) = upper(@Name);               
                """;   
        using var connection = new NpgsqlConnection(ConnectionString);
        SqlMapper.AddTypeHandler(new JsonTypeHandler<List<string>>());
        return await connection.QuerySingleOrDefaultAsync<WebApplication>(query, new
        {
            Username = username,
            Name = name
        });
    }

    public async Task<IEnumerable<WebApplication>> Search(string userId, string query, string sort)
    {
        var sql =
                """
                SELECT 
                    id, 
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
        return await connection.QueryAsync<WebApplication>(sql, new
        {
            user_id = userId,
            query = "%" + query + "%"
        });
    }
    public async Task Update(int id, WebApplication app)
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