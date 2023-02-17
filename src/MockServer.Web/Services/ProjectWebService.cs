using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Enums;
using MockServer.Core.Interfaces;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.Web.Extentions;
using MockServer.Web.Models.Projects;
using MockServer.Web.Models.Requests;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class ProjectWebService : IProjectWebService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IProjectRepository projectRepository;
    private readonly IMapper _mapper;
    private readonly IRequestRepository _requestRepository;
    private readonly IApiKeyService _apiKeyService;

    public ProjectWebService(IHttpContextAccessor contextAccessor,
    IProjectRepository projectRepository,
    IRequestRepository requestRepository,
    IApiKeyService apiKeyService,
    IMapper mapper)
    {
        _requestRepository = requestRepository;
        this._apiKeyService = apiKeyService;
        _mapper = mapper;
        this.contextAccessor = contextAccessor;
        this.projectRepository = projectRepository;
    }

    public async Task<bool> Create(CreateProjectViewModel project)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await projectRepository.Find(user.Id, project.Name);
        if (existing != null)
        {
            return false;
        }
        var mapped = _mapper.Map<Core.Models.Projects.WebApp>(project);
        mapped.UserId = user.Id;
        await projectRepository.Add(mapped);
        return true;
    }

    public async Task Delete(int projectId)
    {
        var project = await projectRepository.Get(projectId);
        if (project != null)
        {
            await projectRepository.Delete(project.Id);
        }
    }

    public async Task<string> GenerateKey(int projectId)
    {
        var project = await projectRepository.Get(projectId);
        Guard.Against.Null(project);
        return _apiKeyService.GenerateApiKey();
    }

    public async Task<ProjectIndexViewModel> GetIndexViewModel(ProjectSearchModel searchModel)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        Enum.TryParse<ProjectAccessibility>(searchModel.Accessibility, out ProjectAccessibility accessibility);
        var projects = await projectRepository.Search(user.Id, searchModel.Query, (int)accessibility, searchModel.Sort);

        var vm = new ProjectIndexViewModel();
        vm.Projects = projects.Select(p => new ProjectIndexItem
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Accessibility = p.Accessibility,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        }).ToList();
        return vm;
    }

    public async Task<ProjectViewViewModel> GetProjectViewViewModel(int projectId)
    {
        var project = await projectRepository.Get(projectId);
        var vm = new ProjectViewViewModel();
        vm.ProjectInfo = _mapper.Map<ProjectIndexItem>(project);
        var requests = await _requestRepository.GetByProjectId(project.Id);
        vm.Requests = _mapper.Map<IEnumerable<RequestIndexItem>>(requests);
        return vm;
    }

    public async Task Rename(int projectId, string newName)
    {
        var project = await projectRepository.Get(projectId);
        if (project != null)
        {
            project.Name = newName;
            await projectRepository.Update(project);
        }
    }

    public async Task SetAccessibility(int projectId, ProjectAccessibility accessibility)
    {
        var project = await projectRepository.Get(projectId);
        if (project != null)
        {
            project.Accessibility = accessibility;
            await projectRepository.Update(project);
        }
    }
}