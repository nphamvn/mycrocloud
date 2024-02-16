using WebApp.Domain.Entities;
using WebApp.Domain.Enums;
using WebApp.Domain.Repositories;

namespace WebApp.Domain.Services;
public interface IAppService {
    Task Create(string userId, App app);
    Task Delete(int id);
    Task Rename(int id, string name);
    Task SetCorsSettings(int id, CorsSettings settings);
    Task SetStatus(int id, AppStatus status);
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
        app.Status = AppStatus.Active;
        app.CorsSettings ??= CorsSettings.Default;
        await _appRepository.Add(userId, app);
    }

    public async Task Delete(int id)
    {
        await _appRepository.Delete(id);
    }

    public async Task Rename(int id, string name)
    {
        var currentApp = await _appRepository.GetByAppId(id);
        currentApp.Name = name;
        await _appRepository.Update(id, currentApp);
    }

    public async Task SetCorsSettings(int id, CorsSettings settings)
    {
        var app = await _appRepository.GetByAppId(id);
        app.CorsSettings = settings;
        await _appRepository.Update(id, app);
    }

    public async Task SetStatus(int id, AppStatus status)
    {
        var app = await _appRepository.GetByAppId(id);
        app.Status = status;
        await _appRepository.Update(id, app);
    }
}
