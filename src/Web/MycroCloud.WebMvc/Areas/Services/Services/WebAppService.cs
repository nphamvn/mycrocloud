using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using WebApp.Api.Grpc;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppService
{
    Task<WebAppModel> Zzz(string userId, string appName);
    Task<WebAppViewViewModel> Get(string name);
    Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel);
    Task Create(WebAppCreateViewModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}

public class WebAppService : BaseService, IWebAppService
{
    private readonly WebApp.Api.Grpc.WebApp.WebAppClient _webAppClient;

    public WebAppService(IHttpContextAccessor contextAccessor
    , WebApp.Api.Grpc.WebApp.WebAppClient webAppClient) : base(contextAccessor)
    {
        _webAppClient = webAppClient;
    }

    public async Task<WebAppModel> Zzz(string userId, string appName)
    {
        throw new NotImplementedException();
    }

    public async Task<WebAppViewViewModel> Get(string name)
    {
        var res = await _webAppClient.GetAsync(new()
        {
            UserId = AuthUser.Id,
            Name = name
        });
        return new()
        {
            Name = res.Name,
            Description = res.Description
        };
    }

    public async Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel)
    {
        var result = await _webAppClient.GetAllAsync(new GetAllWebAppRequest()
        {
            UserId = AuthUser.Id
        });
        var viewModel = new WebAppIndexViewModel
        {
            WebApps = result.WebApps.Select(a => new WebAppIndexItem
            {
                WebAppId = a.Id,
                Name = a.Name
            })
        };
        return viewModel;
    }

    public async Task Create(WebAppCreateViewModel model)
    {
        var result = await _webAppClient.CreateAsync(new CreateWebAppRequest()
        {
            UserId = AuthUser.Id,
            Name = model.Name
        });
    }
    private async Task ValidateCreate(WebAppCreateViewModel model, ModelStateDictionary modelState)
    {

    }

    public async Task Rename(int appId, string name)
    {
        var result = await _webAppClient.RenameAsync(new RenameWebAppRequest()
        {
            Name = name
        });
    }

    public async Task Delete(int appId)
    {
        var result = await _webAppClient.DeleteAsync(new DeleteWebAppRequest()
        {
            Name = appId.ToString()
        });
    }
}