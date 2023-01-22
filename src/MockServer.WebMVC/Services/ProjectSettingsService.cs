using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Entities.Auth;
using MockServer.Core.Enums;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.ProjectSettings;
using MockServer.WebMVC.Models.ProjectSettings.Auth;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

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
        var auth = _mapper.Map<Authentication>(model);
        await _authRepository.Add(project.Id, auth);
    }

    public async Task CreateJwtBearerAuthentication(string name, JwtBearerAuthModel model)
    {
        var project = await GetProjectByName(name);
        var auth = _mapper.Map<Authentication>(model);
        await _authRepository.Add(project.Id, auth);
    }

    public async Task<ApiKeyAuthModel> GetApiKeyAuthModel(string name, int id)
    {
        ApiKeyAuthModel model;
        if (id > 0)
        {
            var auth = await _authRepository.GetAs(id, AuthType.ApiKey);
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
        model.Authentications = await _authRepository.GetByProject(model.Project.Id);
        return model;
    }

    public async Task<IndexModel> GetIndexModel(string name)
    {
        var model = new IndexModel();
        model.Project = await GetProjectByName(name);
        return model;
    }

    public async Task<JwtBearerAuthModel> GetJwtBearerAuthModel(string name, int id)
    {
        JwtBearerAuthModel model;
        if (id > 0)
        {
            var auth = await _authRepository.GetAs(id, AuthType.JwtBearer);
            model = _mapper.Map<JwtBearerAuthModel>(auth);
        }
        else
        {
            model = new JwtBearerAuthModel();
        }
        model.Project = await GetProjectByName(name);
        return model;
    }

    private async Task<MockServer.Core.Models.Project> GetProjectByName(string name)
    {
        var user = _contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var project = await _projectRepository.Get(user.Id, name);
        return _mapper.Map<MockServer.Core.Models.Project>(project);
    }
}
public class ProjectSettingsAuthProfile : Profile
{
    public ProjectSettingsAuthProfile()
    {
        CreateMap<Authentication, JwtBearerAuthModel>().ReverseMap();
        CreateMap<Authentication, ApiKeyAuthModel>().ReverseMap();
        CreateMap<MockServer.Core.Entities.Projects.Project, MockServer.Core.Models.Project>().ReverseMap();
    }
}