using MycroCloud.WebApp;
using MycroCloud.WebMvc.Areas.Services.Models.WebApps;
using static MycroCloud.WebApp.WebAppAuthenticationGrpcService;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppAuthenticationService
{
    Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(string appName);
    Task SaveSettings(int appId, AuthenticationConfigurationViewModel viewModel);
    Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme);
    Task EditJwtBearerScheme(int schemeId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme);
    Task<JwtBearerAuthenticationSchemeSaveViewModel> GetCreateJwtBearerSchemeModel(int appId);
    Task<JwtBearerAuthenticationSchemeSaveViewModel> GetEditJwtBearerSchemeModel(int appId, int schemeId);
    Task<AuthenticationConfigurationViewModel> GetConfigurationsViewModel(int webApplicationId);
}

public class WebAppAuthenticationService(IHttpContextAccessor contextAccessor
, WebAppAuthenticationGrpcServiceClient webAppAuthenticationGrpcServiceClient) : ServiceBaseService(contextAccessor), IWebAppAuthenticationService
{
    private readonly WebAppAuthenticationGrpcServiceClient _webAppAuthenticationClient = webAppAuthenticationGrpcServiceClient;

    public async Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(string appName)
    {
        var vm = new AuthenticationSchemeListViewModel();
        var res = await _webAppAuthenticationClient.GetAllAsync(new () {
            UserId = ServiceOwner.Id,
            AppName = appName
        });
        vm.AuthenticationSchemes = res.Schemes.Select(s => new AuthenticationSchemeIndexItem {
            Id = s.Id,
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

    public async Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme)
    {
        throw new NotImplementedException();
    }

    public async Task EditJwtBearerScheme(int schemeId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtBearerAuthenticationSchemeSaveViewModel> GetCreateJwtBearerSchemeModel(int appId)
    {
        throw new NotImplementedException();
    }

    public async Task<JwtBearerAuthenticationSchemeSaveViewModel> GetEditJwtBearerSchemeModel(int appId, int schemeId)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthenticationConfigurationViewModel> GetConfigurationsViewModel(int webApplicationId)
    {
        throw new NotImplementedException();
    }
}
