using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

public class ProjectService : IProjectService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IProjectRepository projectRepository;
    private readonly IMapper _mapper;
    private readonly IRequestRepository _requestRepository;

    public ProjectService(IHttpContextAccessor contextAccessor,
    IProjectRepository projectRepository,
    IRequestRepository requestRepository,
    IMapper mapper)
    {
        _requestRepository = requestRepository;
        _mapper = mapper;
        this.contextAccessor = contextAccessor;
        this.projectRepository = projectRepository;
    }

    public async Task<bool> Create(CreateProjectViewModel project)
    {
        var user = contextAccessor.HttpContext.User.GetLoggedInUser<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await projectRepository.Find(user.Id, project.Name);
        if (existing != null)
        {
            return false;
        }
        var mapped = _mapper.Map<Core.Entities.Project>(project);
        mapped.UserId = user.Id;
        await projectRepository.Add(mapped);
        return true;
    }

    public async Task<ProjectIndexViewModel> GetIndexViewModel()
    {
        var user = contextAccessor.HttpContext.User.GetLoggedInUser<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var projects = await projectRepository.GetByUserId(user.Id);

        var vm = new ProjectIndexViewModel();
        vm.Projects = projects.Select(p => new ProjectIndexItem
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Accessibility = p.Accessibility
        }).ToList();
        return vm;
    }

    public async Task<ProjectViewViewModel> GetProjectViewViewModel(string name)
    {
        var user = contextAccessor.HttpContext.User.GetLoggedInUser<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var project = await projectRepository.Find(user.Id, name);
        var vm = new ProjectViewViewModel();
        vm.ProjectInfo = _mapper.Map<ProjectIndexItem>(project);
        var requests = await _requestRepository.GetProjectRequests(project.Id);
        vm.Requests = _mapper.Map<IEnumerable<RequestItem>>(requests);
        return vm;
    }

    public async Task Rename(string name, string newName)
    {
        var user = contextAccessor.HttpContext.User.GetLoggedInUser<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var project = await projectRepository.Find(user.Id, name);
        if (project != null)
        {
            project.Name = newName;
            await projectRepository.Update(project);
        }
    }
}