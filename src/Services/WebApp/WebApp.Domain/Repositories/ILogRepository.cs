using WebApp.Domain.Entities;

namespace WebApp.Domain.Repositories;

public interface ILogRepository
{
    Task Add(Log log);
    Task<IQueryable<Log>> Search(int appId);
}
