using Grpc.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppService
{
    Task<WebAppModel> Find(string userName, string appName);
    Task<WebAppViewViewModel> Get(int appId);
    Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel);
    Task Create(WebAppCreateViewModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}

public class WebAppService(IHttpContextAccessor contextAccessor
    , WebAppGrpcService.WebAppGrpcServiceClient webAppGrpcServiceClient) : ServiceBaseService(contextAccessor), IWebAppService
{
    private readonly WebAppGrpcService.WebAppGrpcServiceClient _webAppGrpcServiceClient = webAppGrpcServiceClient;

    public async Task<WebAppModel> Find(string userId, string appName)
    {
        try
        {
            var res = await _webAppGrpcServiceClient.GetAsync(new()
            {
                UserId = userId,
                Name = appName
            });
            return new()
            {
                WebAppId = res.Id,
                UserId = res.UserId,
                WebAppName = res.Name,
                CreatedDate = res.CreatedTime.ToDateTime(),
                UpdatedDate = res.UpdatedTime?.ToDateTime()
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<WebAppViewViewModel> Get(int appId)
    {
        var res = await _webAppGrpcServiceClient.GetByIdAsync(new()
        {
            Id = appId
        });
        return new()
        {
            Name = res.Name,
            Description = res.Description
        };
    }

    public async Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel)
    {
        var result = await _webAppGrpcServiceClient.GetAllAsync(new GetAllWebAppRequest()
        {
            UserId = ServiceOwner.Id
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
        var result = await _webAppGrpcServiceClient.CreateAsync(new CreateWebAppRequest()
        {
            UserId = ServiceOwner.Id,
            Name = model.Name
        });
    }
    private async Task ValidateCreate(WebAppCreateViewModel model, ModelStateDictionary modelState)
    {

    }

    public async Task Rename(int appId, string name)
    {
        var result = await _webAppGrpcServiceClient.RenameAsync(new RenameWebAppRequest()
        {
            Name = name
        });
    }

    public async Task Delete(int appId)
    {
        var result = await _webAppGrpcServiceClient.DeleteAsync(new DeleteWebAppRequest()
        {
            Name = appId.ToString()
        });
    }
}