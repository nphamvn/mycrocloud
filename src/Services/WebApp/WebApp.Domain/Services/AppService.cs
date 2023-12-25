using WebApp.Domain.Entities;
using WebApp.Domain.Repositories;

namespace WebApp.Domain.Services;
public interface IAppService {
    Task Create(string userId, App app);
    Task Rename(int id, string name);
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

    public async Task Rename(int id, string name)
    {
        var currentApp = await _appRepository.GetByAppId(id);
        currentApp.Name = name;
        await _appRepository.Update(id, currentApp);
    }
}
