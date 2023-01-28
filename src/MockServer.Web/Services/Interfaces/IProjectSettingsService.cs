using MockServer.Web.Models.ProjectSettings;
using MockServer.Web.Models.ProjectSettings.Auth;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectSettingsWebService
{
    Task<IndexModel> GetIndexModel(string name);
    Task<AuthIndexModel> GetAuthIndexModel(string name);
    Task CreateJwtBearerAuthentication(string name, JwtBearerAuthModel model);
    Task EditJwtBearerAuthentication(int id, JwtBearerAuthModel model);
    Task CreateApiKeyAuthentication(string name, ApiKeyAuthModel model);
    Task<JwtBearerAuthModel> GetJwtBearerAuthModel(string name, int id);
    Task<ApiKeyAuthModel> GetApiKeyAuthModel(string name, int id);
}