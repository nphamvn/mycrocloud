using MockServer.Core.Databases;
using MockServer.Core.Models.Services;

namespace MockServer.Core.Repositories;

public interface IDatabaseRepository
{
    Task Add(int userId, Database db);
    Task<IEnumerable<Database>> GetAll(int userId);
    Task<Database> Get(int id);
    Task<Database> Find(int userId, string name);
    Task<Database> Find(string username, string name);
    Task Update(int id, Database db);
    Task UpdateData(int id, string data);
    Task UpdateDatabaseUsingService(int id, IList<Service> services);
    Task<IEnumerable<Service>> GetDatabaseUsingService(int id);
    Task Delete(int id);
}
