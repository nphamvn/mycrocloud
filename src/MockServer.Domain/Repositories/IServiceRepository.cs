using MockServer.Domain.Services.Entities;

namespace MockServer.Domain.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetServices(string userid);
}
