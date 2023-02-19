using Database = MockServer.Core.Databases.Database;
namespace MockServer.Core.Repositories;

public interface IDatabaseRespository
{
    Task Add(int userId, Database db);
    Task<Database> Get(int id);
    Task<Database> Find(int userId, string name);
    Task<Database> Find(string username, string name);
}
