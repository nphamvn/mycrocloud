using MycroCloud.WebMvc.Areas.Services.Models.WebApps;

namespace MycroCloud.WebMvc.Areas.Services.Services;
public interface IWebAppAuthenticationService
{
    Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(int appId);
    Task SaveSettings(int appId, AuthenticationConfigurationViewModel viewModel);
    Task AddJwtBearerScheme(int appId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme);
    Task EditJwtBearerScheme(int schemeId, JwtBearerAuthenticationSchemeSaveViewModel authenticationScheme);
    Task<JwtBearerAuthenticationSchemeSaveViewModel> GetCreateJwtBearerSchemeModel(int appId);
    Task<JwtBearerAuthenticationSchemeSaveViewModel> GetEditJwtBearerSchemeModel(int appId, int schemeId);
    Task<AuthenticationConfigurationViewModel> GetConfigurationsViewModel(int webApplicationId);
}

public class WebAppAuthenticationService : BaseService, IWebAppAuthenticationService
{
    public WebAppAuthenticationService(IHttpContextAccessor contextAccessor) : base(contextAccessor)
    {
    }

    public async Task<AuthenticationSchemeListViewModel> GetSchemeListViewModel(int appId)
    {
        throw new NotImplementedException();
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
