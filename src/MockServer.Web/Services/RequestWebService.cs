using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Helpers;
using MockServer.Core.Repositories;
using MockServer.Web.Models.ProjectRequests;
using MockServer.Web.Models.Projects;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class RequestWebService : BaseWebService, IProjectRequestWebService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly IAuthRepository _authRepository;
    private readonly IMapper _mapper;
    public RequestWebService(
        IHttpContextAccessor contextAccessor,
        IRequestRepository requestRepository,
        IProjectRepository projectRepository,
        IAuthRepository authRepository,
        IMapper mapper) : base(contextAccessor)
    {
        _requestRepository = requestRepository;
        _projectRepository = projectRepository;
        _authRepository = authRepository;
        _mapper = mapper;
    }
    public async Task<RequestViewModel> GetRequestOpenViewModel(int requestId)
    {
        var request = await _requestRepository.GetById(requestId);
        var vm = _mapper.Map<RequestViewModel>(request);
        vm.Project = await _projectRepository.Get(request.ProjectId);
        vm.Project.User = AuthUser;
        vm.AuthorizationConfiguration = await GetAuthorization(request.ProjectId, requestId);
        vm.RequestConfiguration = new RequestConfiguration
        {
            //Query = (await _requestRepository.GetRequestQueries(requestId)).ToList(),
            //Headers = (await _requestRepository.GetRequestHeaders(requestId)).ToList(),
            //Body = (await _requestRepository.GetRequestBody(requestId))
        };
        vm.ResponseConfiguration = new ResponseConfiguration
        {
            //Headers = (await _requestRepository.GetResponseHeaders(requestId)).ToList(),
            
        };
        return vm;
    }

    public async Task<int> Create(int projectId, SaveRequestViewModel request)
    {
        var existing = await _requestRepository.Find(projectId, request.Method, request.Path);
        if (existing == null)
        {
            var mapped = _mapper.Map<Core.Models.Requests.Request>(request);
            return await _requestRepository.Create(projectId, mapped);
        }
        else
        {
            return 0;
        }
    }

    public async Task Delete(int id)
    {
        await _requestRepository.Delete(id);
    }

    public async Task<RequestConfiguration> GetFixedRequestConfigViewModel(int id)
    {
        var request = await _requestRepository.GetById(id);
        return _mapper.Map<RequestConfiguration>(request);
    }

    public async Task<SaveRequestViewModel> GetEditRequestViewModel(int requestId)
    {
        var request = await _requestRepository.GetById(requestId);
        var vm = _mapper.Map<SaveRequestViewModel>(request);
        vm.Project = await _projectRepository.Get(request.ProjectId);
        vm.HttpMethods = HttpProtocolExtensions.CommonHttpMethods
                            .Select(m => new SelectListItem(m, m));
        return vm;
    }

    public async Task<bool> ValidateEdit(int id, SaveRequestViewModel request, ModelStateDictionary modelState)
    {
        return modelState.IsValid;
    }

    public async Task Edit(int id, SaveRequestViewModel request)
    {
        var existing = await _requestRepository.GetById(id);
        if (existing != null)
        {
            var mapped = _mapper.Map<Core.Models.Requests.Request>(request);
            await _requestRepository.Update(id, mapped);
        }
    }

    public async Task<AuthorizationConfiguration> GetAuthorization(int projectId, int requestId)
    {
        var authorization = await _authRepository.GetRequestAuthorization(requestId);
        var vm = authorization != null ? _mapper.Map<AuthorizationConfiguration>(authorization)
                                        : new AuthorizationConfiguration();
        vm.AuthenticationSchemeSelectList = await _authRepository.GetProjectAuthenticationSchemes(projectId);
        return vm;
    }

    public async Task AttachAuthorization(int requestId, AuthorizationConfiguration auth)
    {
        var authorization = _mapper.Map<Core.Models.Auth.Authorization>(auth);
        await _authRepository.UpdateRequestAuthorization(requestId, authorization);
    }

    public async Task<IndexViewModel> GetIndexViewModel(int projectId)
    {
        return new IndexViewModel
        {
            Project = _mapper.Map<Project>(await _projectRepository.Get(projectId)),
            Requests = _mapper.Map<IEnumerable<RequestIndexItem>>(await _requestRepository.GetByProjectId(projectId))
        };
    }

    public Task<bool> ValidateCreate(int projectId, SaveRequestViewModel request, ModelStateDictionary modelState)
    {
        return Task.FromResult(modelState.IsValid);
    }

    public Task SaveRequestConfiguration(int requestId, RequestConfiguration config)
    {
        throw new NotImplementedException();
    }

    public async Task<SaveRequestViewModel> GetCreateRequestViewModel(int projectId)
    {
        var vm = new SaveRequestViewModel();
        vm.Project = await _projectRepository.Get(projectId);
        vm.Project.User = AuthUser;
        vm.HttpMethods = HttpProtocolExtensions.CommonHttpMethods
                            .Select(m => new SelectListItem(m, m));
        return vm;
    }
}
