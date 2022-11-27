using Dapper;
using Microsoft.Data.Sqlite;
using MockServer.Core.Entities;
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
                        Accessibility
                    )
                    VALUES (
                        @UserId,
                        @Name,
                        @Description,
                        @Accessibility
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

    public async Task<Project> Find(int userId, string projectName)
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
                    Name = @Name;               
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Project>(query, new
        {
            UserId = userId,
            Name = projectName
        });
    }

    public async Task<Project> GetById(int id)
    {
        var query =
                """
                    SELECT Id,
                        UserId,
                        Name,
                        Description,
                        PrivateAccess,
                        PrivateKey
                    FROM Project
                    WHERE Id = @id;
                """;
        using var connection = new SqliteConnection(_connectionString);
        return await connection.QuerySingleOrDefaultAsync<Project>(query, new
        {
            id = id
        });
    }

    public async Task<IEnumerable<Project>> GetByUserId(int userId)
    {
        var query =
                """
                SELECT Id,
                    Name,
                    Description,
                    Accessibility,
                    PrivateKey
                FROM Project
                WHERE UserId = @UserId;
                """;

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<Project>(query, new
        {
            UserId = userId
        });
    }

    public async Task Update(Project project)
    {
        var query =
                """
                UPDATE Project
                SET
                    Name = @Name,
                    Description = @Description,
                    Accessibility = @Accessibility,
                    PrivateKey = @PrivateKey
                WHERE Id = @Id;
                """;
        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(query, new
        {
            Id= project.Id,
            Name = project.Name,
            Description = project.Description,
            Accessibility = (int)project.Accessibility,
            PrivateKey = project.PrivateKey ?? null
        });
    }
}