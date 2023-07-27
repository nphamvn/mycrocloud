using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppService
{
    Task<WebAppModel> Zzz(string userId, string appName);
    Task<WebAppViewViewModel> Get(int appId);
    Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel);
    Task Create(WebAppCreateViewModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}

public class WebAppService : BaseService, IWebAppService
{
    public WebAppService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
        
    }

    public async Task<WebAppModel> Zzz(string userId, string appName)
    {
        throw new NotImplementedException();
    }

    public async Task<WebAppViewViewModel> Get(int appId)
    {
        throw new NotImplementedException();
    }

    public async Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel)
    {
        throw new NotImplementedException();
    }

    public async Task Create(WebAppCreateViewModel app)
    {
        throw new NotImplementedException();
    }

    public async Task Rename(int appId, string name)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(int appId)
    {
        throw new NotImplementedException();
    }
}