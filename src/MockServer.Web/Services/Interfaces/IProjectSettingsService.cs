using MockServer.WebMVC.Models.ProjectSettings;
using MockServer.WebMVC.Models.ProjectSettings.Auth;

namespace MockServer.WebMVC.Services.Interfaces;

public interface IProjectSettingsWebService
{
    Task<IndexModel> GetIndexModel(string name);
    Task<AuthIndexModel> GetAuthIndexModel(string name);
    Task CreateJwtBearerAuthentication(string name, JwtBearerAuthModel model);
    Task CreateApiKeyAuthentication(string name, ApiKeyAuthModel model);
    Task<JwtBearerAuthModel> GetJwtBearerAuthModel(string name, int id);
    Task<ApiKeyAuthModel> GetApiKeyAuthModel(string name, int id);
}