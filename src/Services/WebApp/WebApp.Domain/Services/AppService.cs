using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Domain.Services;
public interface IAppService {
    Task Create(string userId, App app);
}

public class AppService : IAppService
{
    private readonly IAppRepository _appRepository;

    public AppService(IAppRepository appRepository)
    {
        _appRepository = appRepository;
    }
    public async Task Create(string userId, App app)
    {
        await _appRepository.Add(userId, app);
    }
}
