using System.Security.Claims;
using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Models.Auth;
using MockServer.Core.Interfaces;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.Core.Services;
using MockServer.Web.Extentions;
using MockServer.Web.Models.ProjectSettings;
using MockServer.Web.Models.ProjectSettings.Auth;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class ProjectSettingsWebService : IProjectSettingsWebService
{
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IAuthRepository _authRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IMapper _mapper;

    public ProjectSettingsWebService(
        IHttpContextAccessor contextAccessor,
        IAuthRepository authRepository,
        IProjectRepository projectRepository,
        IMapper mapper)
    {
        _contextAccessor = contextAccessor;
        _authRepository = authRepository;
        _projectRepository = projectRepository;
        _mapper = mapper;
    }

    public async Task CreateApiKeyAuthentication(string name, ApiKeyAuthModel model)
    {
        var project = await GetProjectByName(name);
        var auth = _mapper.Map<AppAuthentication>(model);
        await _authRepository.AddProjectAuthenticationScheme(project.Id, auth);
    }

    public async Task CreateJwtBearerAuthentication(string name, JwtBearerAuthModel model)
    {
        var project = await GetProjectByName(name);
        var auth = _mapper.Map<AppAuthentication>(model);
        await _authRepository.AddProjectAuthenticationScheme(project.Id, auth);
    }

    public async Task EditJwtBearerAuthentication(int id, JwtBearerAuthModel model)
    {
        var auth = _mapper.Map<AppAuthentication>(model);
        await _authRepository.UpdateProjectAuthenticationScheme(id, auth);
    }

    public async Task<JwtBearerTokenGenerateModel> GenerateJwtBearerToken(string projectName, int schemeId, JwtBearerTokenGenerateModel model)
    {
        var auth = await _authRepository.GetAuthenticationScheme<JwtBearerAuthenticationOptions>(schemeId);
        var options = (JwtBearerAuthenticationOptions)auth.Options;
        var claims = model.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
        IJwtBearerTokenService jwtBearerTokenService = new JwtBearerTokenService();
        model.Token = jwtBearerTokenService.GenerateToken(options, claims);
        return model;
    }

    public async Task<ApiKeyAuthModel> GetApiKeyAuthModel(string name, int id)
    {
        ApiKeyAuthModel model;
        if (id > 0)
        {
            //var auth = await _authRepository.GetAuthenticationScheme(id, AuthenticationType.ApiKey);
            var auth = await _authRepository.GetAuthenticationScheme<ApiKeyAuthenticationOptions>(id);
            model = _mapper.Map<ApiKeyAuthModel>(auth);
        }
        else
        {
            model = new ApiKeyAuthModel();
        }
        model.Project = await GetProjectByName(name);
        return model;
    }

    public async Task<AuthIndexModel> GetAuthIndexModel(string name)
    {
        var model = new AuthIndexModel();
        model.Project = await GetProjectByName(name);
        model.AuthenticationSchemes = await _authRepository.GetProjectAuthenticationSchemes(model.Project.Id);
        return model;
    }

    public async Task<IndexModel> GetIndexModel(int projectId)
    {
        var model = new IndexModel();
        var project = await _projectRepository.Get(projectId);
        model.Project = _mapper.Map<MockServer.Web.Models.Projects.Project>(project);
        return model;
    }

    public async Task<JwtBearerAuthModel> GetJwtBearerAuthModel(string name, int id)
    {
        JwtBearerAuthModel model;
        if (id > 0)
        {
            //var auth = await _authRepository.GetAuthenticationScheme(id, AuthenticationType.JwtBearer);
            //var auth = await _authRepository.GetAuthenticationScheme(id);
            var auth = await _authRepository.GetAuthenticationScheme<JwtBearerAuthenticationOptions>(id);
            model = _mapper.Map<JwtBearerAuthModel>(auth);
        }
        else
        {
            model = new JwtBearerAuthModel();
        }
        model.Project = await GetProjectByName(name);
        return model;
    }

    public async Task SaveAuthIndexModel(string name, AuthIndexModel model)
    {
        var user = _contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var project = await _projectRepository.Get(user.Id, name);
        await _authRepository.ActivateProjectAuthenticationSchemes(project.Id, model.AuthenticationSchemes.Select(s => s.Id).ToList());
    }

    private async Task<MockServer.Web.Models.Projects.Project> GetProjectByName(string name)
    {
        var user = _contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var project = await _projectRepository.Get(user.Id, name);
        return _mapper.Map<MockServer.Web.Models.Projects.Project>(project);
    }
}
public class ProjectSettingsAuthProfile : Profile
{
    public ProjectSettingsAuthProfile()
    {
        CreateMap<AppAuthentication, JwtBearerAuthModel>().ReverseMap();
        CreateMap<AppAuthentication, ApiKeyAuthModel>().ReverseMap();
        CreateMap<MockServer.Core.Models.Projects.Project, MockServer.Web.Models.Projects.Project>().ReverseMap();
    }
}