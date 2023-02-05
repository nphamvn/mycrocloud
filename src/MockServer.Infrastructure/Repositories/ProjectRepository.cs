using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Models.Auth;
using MockServer.Core.Models.Projects;
using MockServer.Core.Repositories;
using MockServer.Core.Settings;

namespace MockServer.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly string _connectionString;
    public ProjectRepository(GlobalSettings settings)
    {
        _connectionString = settings.Sqlite.ConnectionString;
    }

    public async Task Add(Project project)
    {
        var query =
                """
                INSERT INTO Project (
                        UserId,
                        Name,
                        Description,
                        Accessibility,
                        CreatedAt
                    )
                    VALUES (
                        @UserId,
                        @Name,
                        @Description,
                        @Accessibility,
                        datetime('now','localtime')
                    );
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            UserId = project.UserId,
            Name = project.Name,
            Description = project.Description,
            Accessibility = (int)project.Accessibility
        });
    }

    public async Task Delete(int id)
    {
        var query =
                """
                DELETE FROM Project
                    WHERE Id = @Id;              
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = id
        });
    }

    public async Task<Project> Get(int userId, string projectName)
    {
        var query =
                """
                SELECT Id,
                    Name,
                    Description,
                    Accessibility,
                    PrivateKey
                FROM Project
                WHERE UserId = @UserId AND 
                    upper(Name) = upper(@Name);               
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Project>(query, new
        {
            UserId = userId,
            Name = projectName
        });
    }

    public async Task<Project> Get(int id)
    {
        var query =
                """
                    SELECT 
                        Id,
                        UserId,
                        Name,
                        Description,
                        PrivateAccess,
                        PrivateKey
                    FROM 
                        Project
                    WHERE 
                        Id = @id;
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Project>(query, new
        {
            id = id
        });
    }

    public async Task<Project> Get(string username, string name)
    {
        var query =
                """
                SELECT 
                    p.Id,
                    p.Name,
                    p.Description,
                    p.Accessibility,
                    p.PrivateKey
                FROM Project p
                INNER JOIN Users u ON p.UserId = u.Id
                WHERE upper(u.Username) = upper(@Username) AND 
                    upper(Name) = upper(@Name);               
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Project>(query, new
        {
            Username = username,
            Name = name
        });
    }

    public async Task<IEnumerable<Project>> Search(int userId, string query, int accessibility, string sort)
    {
        var sql =
                """
                SELECT Id,
                    Name,
                    Description,
                    Accessibility,
                    PrivateKey,
                    CreatedAt,
                    UpdatedAt
                FROM Project
                WHERE UserId = @UserId
                """;
        List<string> conditions = new();
        if (!string.IsNullOrEmpty(query))
        {
            conditions.Add("Name LIKE @Query OR Description LIKE @Query");
        }
        if (accessibility != 0)
        {
            conditions.Add("Accessibility = @Accessibility");
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
        return await connection.QueryAsync<Project>(sql, new
        {
            UserId = userId,
            Query = "%" + query + "%",
            Accessibility = accessibility
        });
    }

    public Task<JwtBearerAuthenticationOptions> GetJwtHandlerConfiguration(int id)
    {
        throw new NotImplementedException();
    }

    public async Task Update(Project project)
    {
        var query =
                """
                UPDATE 
                    Project
                SET
                    Name = @Name,
                    Description = @Description,
                    Accessibility = @Accessibility,
                    UpdatedAt = datetime('now','localtime')
                WHERE Id = @Id;
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Accessibility = (int)project.Accessibility
        });
    }
}