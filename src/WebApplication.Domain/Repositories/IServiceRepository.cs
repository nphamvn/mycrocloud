using WebApplication.Domain.Services.Entities;

namespace WebApplication.Domain.Repositories;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetServices(string userid);
}
