using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Enums;
using MockServer.Core.Helpers;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

public class RequestWebService : BaseWebService, IRequestWebService
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
    public async Task<RequestOpenViewModel> GetRequestOpenViewModel(string projectName, int requestId)
    {
        var request = await _requestRepository.Get(AuthUser.Id, projectName, requestId);
        var vm = _mapper.Map<RequestOpenViewModel>(request);
        vm.Configuration = await this.GetRequestConfiguration(request);
        vm.Username = AuthUser.Username;
        vm.ProjectName = projectName;
        return vm;
    }
    private async Task<RequestConfiguration> GetRequestConfiguration(Core.Entities.Requests.Request request)
    {
        RequestConfiguration ret = null;
        switch (request.Type)
        {
            case RequestType.Fixed:
                ret = new FixedRequestConfiguration
                {
                    RequestParams = (await _requestRepository.GetRequestParams(request.Id)).ToList(),
                    RequestHeaders = (await _requestRepository.GetRequestHeaders(request.Id)).ToList(),
                    RequestBody = await _requestRepository.GetRequestBody(request.Id),
                    ResponseHeaders = (await _requestRepository.GetResponseHeaders(request.Id)).ToList(),
                    Response = await _requestRepository.GetResponse(request.Id)
                };
                break;
            default:
                break;
        }
        return ret;
    }
    public async Task<int> Create(string projectName, CreateUpdateRequestViewModel request)
    {
        var existing = await _requestRepository.Get(AuthUser.Id, projectName, request.Method, request.Path);
        if (existing == null)
        {
            var mapped = _mapper.Map<Core.Entities.Requests.Request>(request);
            return await _requestRepository.Create(AuthUser.Id, projectName, mapped);
        }
        else
        {
            return 0;
        }
    }

    public async Task Delete(string projectname, int id)
    {
        await _requestRepository.Delete(AuthUser.Id, projectname, id);
    }

    public async Task<RequestItem> Get(string projectname, int id)
    {
        var request = await _requestRepository.Get(AuthUser.Id, projectname, id);
        return _mapper.Map<RequestItem>(request);
    }

    public async Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(string projectname, int id)
    {
        var request = await _requestRepository.Get(AuthUser.Id, projectname, id);
        return _mapper.Map<FixedRequestConfigViewModel>(request);
    }

    public async Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config)
    {
        var mapped = _mapper.Map<Core.Entities.Requests.FixedRequest>(config);
        if (fields.Contains(nameof(config.RequestParams)))
        {
            await _requestRepository.UpdateRequestParams(id, mapped);
        }

        if (fields.Contains(nameof(config.RequestHeaders)))
        {
            await _requestRepository.UpdateRequestHeaders(id, mapped);
        }

        if (fields.Contains(nameof(config.RequestBody)))
        {
            await _requestRepository.UpdateRequestBody(id, mapped);
        }

        if (fields.Contains(nameof(config.ResponseHeaders)))
        {
            await _requestRepository.UpdateResponseHeaders(id, mapped);
        }
        if (fields.Contains(nameof(config.Response)))
        {
            await _requestRepository.UpdateResponse(id, mapped);
        }
    }

    public async Task<CreateUpdateRequestViewModel> GetRequestModel(string projectName, int requestId)
    {
        var request = await _requestRepository.Get(AuthUser.Id, projectName, requestId);
        var vm = _mapper.Map<CreateUpdateRequestViewModel>(request);
        return vm;
    }

    public async Task<bool> ValidateEdit(string projectname, int id, CreateUpdateRequestViewModel request, ModelStateDictionary modelState)
    {
        return modelState.IsValid;
    }

    public async Task Edit(string projectName, int id, CreateUpdateRequestViewModel request)
    {
        var existing = await _requestRepository.Get(AuthUser.Id, projectName, id);
        if (existing != null)
        {
            var mapped = _mapper.Map<Core.Entities.Requests.Request>(request);
            await _requestRepository.Update(AuthUser.Id, projectName, id, mapped);
        }
    }

    public async Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel(string projectName)
    {
        var project = await _projectRepository.Get(AuthUser.Id, projectName);
        var vm = new CreateUpdateRequestViewModel();
        vm.HttpMethods = HttpProtocolExtensions.CommonHttpMethods
                            .Select(m => new SelectListItem(m, m));
        var authenticationSchemes = await _authRepository.GetByProject(project.Id);
        vm.AuthenticationSchemes = authenticationSchemes.Select(s => new SelectListItem(s.Id.ToString(), s.SchemeName));
        return vm;
    }
}
