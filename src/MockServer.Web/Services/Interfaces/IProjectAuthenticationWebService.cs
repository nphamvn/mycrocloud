using MockServer.Web.Models.ProjectAuthentication;
using MockServer.Web.Models.ProjectAuthentication.JwtBearer;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectAuthenticationWebService
{
    Task<IndexViewModel> GetIndexViewModel(int projectId);
    Task SaveConfigurations(int projectId, IndexViewModel model);
    Task AddJwtBearerScheme(int projectId, JwtBearerSchemeViewModel model);
    Task EditJwtBearerScheme(int schemeId, JwtBearerSchemeViewModel model);
    Task<JwtBearerSchemeViewModel> GetJwtBearerScheme(int projectId, int schemeId);
}
