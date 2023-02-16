using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using MockServer.Core.Enums;
using MockServer.Core.Helpers;
using MockServer.Core.Models.Requests;
using MockServer.Core.Repositories;
using MockServer.Web.Models.Requests;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

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
    public async Task<RequestOpenViewModel> GetRequestOpenViewModel(int projectId, int requestId)
    {
        var request = await _requestRepository.GetById(requestId);
        var vm = _mapper.Map<RequestOpenViewModel>(request);
        vm.Configuration = await this.GetRequestConfiguration(request);
        return vm;
    }
    private async Task<RequestConfiguration> GetRequestConfiguration(Core.Models.Requests.Request request)
    {
        RequestConfiguration ret = null;
        switch (request.Type)
        {
            case RequestType.Fixed:
                var req = await _requestRepository.GetById(request.Id);
                ret = new FixedRequestConfiguration
                {
                    RequestParams = req.Queries ?? new List<RequestQuery>(),
                    RequestHeaders = req.Headers?? new List<RequestHeader>(),
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
    public async Task<int> Create(int projectId, CreateUpdateRequestViewModel request)
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

    public async Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(int id)
    {
        var request = await _requestRepository.GetById(id);
        return _mapper.Map<FixedRequestConfigViewModel>(request);
    }

    public async Task SaveFixedRequestConfig(int id, string[] fields, FixedRequestConfigViewModel config)
    {
        var mapped = _mapper.Map<Core.Models.Requests.FixedRequest>(config);
        if (fields.Contains(nameof(config.RequestParams)))
        {
            await _requestRepository.UpdateRequestQuery(id, mapped.RequestParams);
        }

        if (fields.Contains(nameof(config.RequestHeaders)))
        {
            await _requestRepository.UpdateRequestHeader(id, mapped.RequestHeaders);
        }

        if (fields.Contains(nameof(config.RequestBody)))
        {
            await _requestRepository.UpdateRequestBody(id, mapped.RequestBody);
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

    public async Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel(int requestId)
    {
        var request = await _requestRepository.GetById(requestId);
        var vm = _mapper.Map<CreateUpdateRequestViewModel>(request);
        vm.HttpMethods = HttpProtocolExtensions.CommonHttpMethods
                            .Select(m => new SelectListItem(m, m));
        return vm;
    }

    public async Task<bool> ValidateEdit(int id, CreateUpdateRequestViewModel request, ModelStateDictionary modelState)
    {
        return modelState.IsValid;
    }

    public async Task Edit(int id, CreateUpdateRequestViewModel request)
    {
        var existing = await _requestRepository.GetById(id);
        if (existing != null)
        {
            var mapped = _mapper.Map<Core.Models.Requests.Request>(request);
            await _requestRepository.Update(id, mapped);
        }
    }

    public async Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel()
    {
        var vm = new CreateUpdateRequestViewModel();
        vm.HttpMethods = HttpProtocolExtensions.CommonHttpMethods
                            .Select(m => new SelectListItem(m, m));
        return vm;
    }

    public async Task<AuthorizationConfigViewModel> GetAuthorizationConfigViewModel(int projectId, int requestId)
    {
        var authorization = await _authRepository.GetRequestAuthorization(requestId);
        var vm = authorization != null ? _mapper.Map<AuthorizationConfigViewModel>(authorization)
                                        : new AuthorizationConfigViewModel();
        vm.AuthenticationSchemeSelectList = await _authRepository.GetProjectAuthenticationSchemes(projectId);
        return vm;
    }

    public async Task ConfigureRequestAuthorization(int requestId, AuthorizationConfigViewModel auth)
    {
        var authorization = _mapper.Map<Core.Models.Auth.Authorization>(auth);
        await _authRepository.UpdateRequestAuthorization(requestId, authorization);
    }
}
