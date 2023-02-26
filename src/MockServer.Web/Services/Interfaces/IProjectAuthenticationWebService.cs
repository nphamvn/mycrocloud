using MockServer.Web.Models.WebApplications.Authentications;
using MockServer.Web.Models.WebApplications.Authentications.JwtBearer;

namespace MockServer.Web.Services;

public interface IWebApplicationAuthenticationWebService
{
    Task<AuthenticationIndexModel> GetIndexViewModel(int appId);
    Task SaveConfigurations(int appId, AuthenticationIndexModel model);
    Task AddJwtBearerScheme(int appId, JwtBearerSchemeSaveModel scheme);
    Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeSaveModel scheme);
    Task<JwtBearerSchemeSaveModel> GetJwtBearerScheme(int schemeId);
}
