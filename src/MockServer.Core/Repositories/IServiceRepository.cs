using MockServer.Core.Services;

namespace MockServer.Core.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetServices(int userid);
}
