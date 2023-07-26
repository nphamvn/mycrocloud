using MicroCloud.Web.Models.WebApplications.Authentications;
using MicroCloud.Web.Models.WebApplications.Authentications.JwtBearer;

namespace MicroCloud.Web.Services;

public interface IWebApplicationAuthenticationWebService
{
    Task<AuthenticationSettingsModel> GetAuthenticationSettingsModel(int appId);
    Task<AuthenticationIndexModel> GetIndexViewModel(int appId);
    Task SaveSettings(int appId, AuthenticationSettingsModel model);
    Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveModel scheme);
    Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeSaveModel scheme);
    Task<JwtBearerSchemeSaveModel> GetCreateJwtBearerSchemeModel(int appId);
    Task<JwtBearerSchemeSaveModel> GetEditJwtBearerSchemeModel(int appId, int schemeId);
}
