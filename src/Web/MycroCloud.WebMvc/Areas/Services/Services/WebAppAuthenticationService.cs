using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using static MycroCloud.WebApp.WebAppAuthenticationGrpcService;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppAuthenticationService
{
    Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(int appId);
    Task SaveSettings(int appId, AuthenticationConfigurationViewModel viewModel);
    Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveViewModel authenticationScheme);
    Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeSaveViewModel authenticationScheme);
    Task<JwtBearerSchemeSaveViewModel> GetCreateJwtBearerSchemeModel(int appId);
    Task<JwtBearerSchemeSaveViewModel> GetEditJwtBearerSchemeModel(int appId, int schemeId);
    Task<AuthenticationConfigurationViewModel> GetConfigurationsViewModel(int appId);
}

public class WebAppAuthenticationService(
WebAppAuthenticationGrpcServiceClient webAppAuthenticationGrpcServiceClient) : IWebAppAuthenticationService
{
    public async Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(int appId)
    {
        var vm = new AuthenticationSchemeListViewModel();
        var res = await webAppAuthenticationGrpcServiceClient.GetAllAsync(new () {
            
        });
        vm.AuthenticationSchemes = res.Schemes.Select(s => new AuthenticationSchemeIndexItem {
            Id = s.SchemeId,
            Type = (AuthenticationSchemeType)(int)s.Type,
            Name = s.Name,
            DisplayName = s.DisplayName
        });
        return vm;
    }

    public async Task SaveSettings(int appId, AuthenticationConfigurationViewModel viewModel)
    {
        throw new NotImplementedException();
    }

    public async Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveViewModel model)
    {
        await webAppAuthenticationGrpcServiceClient.CreateJwtBearerSchemeAsync(new CreateJwtBearerSchemeRequest {
            AppId = appId,
            Name = model.Name,
            DisplayName = model.DisplayName,
            Description = model.Description,
            Authority = model.Authority,
            Audience = model.Audience
        });
    }

    public async Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeSaveViewModel authenticationScheme)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtBearerSchemeSaveViewModel> GetCreateJwtBearerSchemeModel(int appId)
    {
        return new JwtBearerSchemeSaveViewModel();
    }

    public async Task<JwtBearerSchemeSaveViewModel> GetEditJwtBearerSchemeModel(int appId, int schemeId)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationConfigurationViewModel> GetConfigurationsViewModel(int webApplicationId)
    {
        throw new NotImplementedException();
    }
}
