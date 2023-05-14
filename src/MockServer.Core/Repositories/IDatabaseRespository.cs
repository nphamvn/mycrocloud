using MockServer.Core.Databases;
using MockServer.Core.Services;

namespace MockServer.Core.Repositories;

public interface IDatabaseRepository
{
    Task Add(string userId, Database db);
    Task<IEnumerable<Database>> GetAll(string userId);
    Task<Database> Get(int id);
    Task<Database> FindByUserId(string userId, string name);
    Task<Database> FindByUsername(string username, string name);
    Task Update(int id, Database db);
    Task UpdateData(int id, string data);
    Task UpdateDatabaseUsingService(int id, IList<Service> services);
    Task<IEnumerable<Service>> GetDatabaseUsingService(int id);
    Task Delete(int id);
}
