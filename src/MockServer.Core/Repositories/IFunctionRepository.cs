using MockServer.Core.Functions;

namespace MockServer.Core.Repositories;

public interface IFunctionRepository
{
    Task<IEnumerable<Function>> GetAll(string userId);
    Task<Function> Get(int id);
    Task<int> Add(string userId, Function function);
    Task Update(int id, Function function);
    Task Delete(int id);
    Task<IEnumerable<Runtime>> GetAllRuntimes();
}
