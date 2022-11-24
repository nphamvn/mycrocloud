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
    public Task<Project> Find(string email, string projectName)
    {
        throw new NotImplementedException();
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
}