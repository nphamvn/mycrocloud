using Ardalis.GuardClauses;
using AutoMapper;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

public class RequestService : IRequestService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper mapper;

    public RequestService(IHttpContextAccessor contextAccessor,
    IRequestRepository requestRepository,
    IMapper mapper)
    {
        this.contextAccessor = contextAccessor;
        _requestRepository = requestRepository;
        this.mapper = mapper;
    }
    public async Task<int> Create(string projectName, CreateRequestViewModel request)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await _requestRepository.FindRequest(user.Id, projectName, request.Method, request.Path);
        if (existing == null)
        {
            var mapped = mapper.Map<Core.Entities.Requests.Request>(request);
            return await _requestRepository.Create(user.Id, projectName, mapped);
        }
        else
        {
            return 0;
        }
    }

    public async Task Delete(string projectname, int id)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        await _requestRepository.Delete(user.Id, projectname, id);
    }

    public async Task<RequestItem> Get(string projectname, int id)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var request = await _requestRepository.Get(user.Id, projectname, id);
        return mapper.Map<RequestItem>(request);
    }

    public async Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(string projectname, int id)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var request = await _requestRepository.GetFixedRequestConfig(user.Id, projectname, id);

        return mapper.Map<FixedRequestConfigViewModel>(request);
    }

    public async Task SaveFixedRequestConfig(string projectname, int id, FixedRequestConfigViewModel config)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var mapped = mapper.Map<Core.Entities.Requests.FixedRequest>(config);
        await _requestRepository.SaveFixedRequestConfig(user.Id, projectname, id, mapped);
    }
}
