using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface ILogRepository
{
    Task Add(Log log);
    Task DeleteByRouteId(int id);
    Task<IQueryable<Log>> Search(int appId);
}
