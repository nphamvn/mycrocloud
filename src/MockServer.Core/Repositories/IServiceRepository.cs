using MockServer.Core.Models.Services;

namespace MockServer.Core.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetServices(int userid);
}
