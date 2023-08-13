using Grpc.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppService
{
    Task<WebAppModel> FindByUserIdAndAppName(string userName, string appName);
    Task<WebAppViewViewModel> GetViewViewModel(int appId);
    Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel, string userId);
    Task Create(WebAppCreateViewModel app, string userId);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}

public class WebAppService(WebAppGrpcService.WebAppGrpcServiceClient webAppGrpcServiceClient) :IWebAppService
{
    public async Task<WebAppModel> FindByUserIdAndAppName(string userId, string appName)
    {
        try
        {
            var res = await webAppGrpcServiceClient.GetAppByUserIdAndAppNameAsync(new()
            {
                UserId = userId,
                AppName = appName
            });
            return new()
            {
                WebAppId = res.AppId,
                WebAppName = res.AppName,
                CreatedDate = res.CreatedTime.ToDateTime(),
                UpdatedDate = res.UpdatedTime?.ToDateTime()
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task<WebAppViewViewModel> GetViewViewModel(int appId)
    {
        var res = await webAppGrpcServiceClient.GetAppByIdAsync(new()
        {
            AppId = appId
        });
        return new()
        {
            Name = res.AppName,
            Description = res.Description
        };
    }

    public async Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel, string userId)
    {
        var result = await webAppGrpcServiceClient.ListAppsByUserIdAsync(new ()
        {
            UserId = userId
        });
        var viewModel = new WebAppIndexViewModel
        {
            WebApps = result.Apps.Select(a => new WebAppIndexItem
            {
                WebAppId = a.AppId,
                Name = a.AppName
            })
        };
        return viewModel;
    }

    public async Task Create(WebAppCreateViewModel model, string userId)
    {
        await webAppGrpcServiceClient.CreateAppAsync(new ()
        {
            UserId = userId,
            AppName = model.Name
        });
    }
    private async Task ValidateCreate(WebAppCreateViewModel model, ModelStateDictionary modelState)
    {

    }

    public async Task Rename(int appId, string name)
    {
        var result = await webAppGrpcServiceClient.RenameAppAsync(new ()
        {
            AppId = appId,
            AppName = name
        });
    }

    public async Task Delete(int appId)
    {
        await webAppGrpcServiceClient.DeleteAppAsync(new()
        {
            AppId = appId
        });
    }
}