using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;
using MockServer.Core.WebApplications;

namespace MockServer.Infrastructure.Repositories.Sqlite;

public class WebApplicationRepository : IWebApplicationRepository
{
    private readonly string _connectionString;
    public WebApplicationRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }

    public async Task Add(string userId, WebApplication app)
    {
        var query =
                """
                INSERT INTO
                    WebApplication (
                        UserId,
                        Name,
                        Description,
                        CreatedAt
                    ) VALUES (
                        @UserId,
                        @Name,
                        @Description,
                        datetime('now','localtime')
                    );
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            UserId = userId,
            Name = app.Name,
            Description = app.Description
        });
    }

    public async Task Delete(int id)
    {
        var query =
                """
                DELETE FROM
                    WebApplication
                WHERE
                    Id = @Id;              
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = id
        });
    }

    public async Task<WebApplication> FindByUserId(string userId, string name)
    {
        var query =
                """
                SELECT 
                    Id,
                    Name,
                    Description,
                    Accessibility
                FROM
                    WebApplication
                WHERE 
                    UserId = @UserId AND 
                    upper(Name) = upper(@Name);               
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<WebApplication>(query, new
        {
            UserId = userId,
            Name = name
        });
    }

    public async Task<WebApplication> Get(int id)
    {
        var query =
                """
                    SELECT 
                        Id,
                        UserId,
                        Name,
                        Description
                    FROM 
                        WebApplication
                    WHERE 
                        Id = @id;
                """;
        using var connection = new SqliteConnection(_connectionString);
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
        using var connection = new SqliteConnection(_connectionString);
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
                    Id,
                    Name,
                    Description,
                    Accessibility,
                    CreatedAt,
                    UpdatedAt
                FROM
                    WebApplication
                WHERE
                    UserId = @UserId
                """;
        List<string> conditions = new();
        if (!string.IsNullOrEmpty(query))
        {
            conditions.Add("Name LIKE @Query OR Description LIKE @Query");
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
            sql += " ORDER BY UpdatedAt DESC";
        }

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<WebApplication>(sql, new
        {
            UserId = userId,
            Query = "%" + query + "%"
        });
    }
    public async Task Update(int id, WebApplication app)
    {
        var query =
                """
                UPDATE 
                    WebApplication
                SET
                    Name = @Name,
                    Description = @Description,
                    UpdatedAt = datetime('now','localtime')
                WHERE
                    Id = @Id;
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = id,
            Name = app.Name,
            Description = app.Description
        });
    }
}