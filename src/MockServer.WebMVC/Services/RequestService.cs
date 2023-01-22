using Ardalis.GuardClauses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Core.Enums;
using MockServer.Core.Models;
using MockServer.Core.Repositories;
using MockServer.WebMVC.Extentions;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;
using MockServer.WebMVC.Services.Interfaces;

namespace MockServer.WebMVC.Services;

public class RequestWebService : IRequestWebService
{
    private readonly IHttpContextAccessor contextAccessor;
    private readonly IRequestRepository _requestRepository;
    private readonly IMapper _mapper;

    public RequestWebService(IHttpContextAccessor contextAccessor,
    IRequestRepository requestRepository,
    IMapper mapper)
    {
        this.contextAccessor = contextAccessor;
        _requestRepository = requestRepository;
        _mapper = mapper;
    }
    public async Task<RequestOpenViewModel> GetRequestOpenViewModel(string projectName, int requestId)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var request = await _requestRepository.Get(user.Id, projectName, requestId);
        var vm = _mapper.Map<RequestOpenViewModel>(request);
        vm.Configuration = await this.GetRequestConfiguration(request);
        vm.Username = user.Username;
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
    public async Task<int> Create(string projectName, CreateUpdateRequestModel request)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await _requestRepository.Get(user.Id, projectName, request.Method, request.Path);
        if (existing == null)
        {
            var mapped = _mapper.Map<Core.Entities.Requests.Request>(request);
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
        return _mapper.Map<RequestItem>(request);
    }

    public async Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(string projectname, int id)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var request = await _requestRepository.Get(user.Id, projectname, id);

        return _mapper.Map<FixedRequestConfigViewModel>(request);
    }

    public async Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
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

    public async Task<CreateUpdateRequestModel> GetRequestViewModel(string projectName, int requestId)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));
        var request = await _requestRepository.Get(user.Id, projectName, requestId);
        var vm = _mapper.Map<CreateUpdateRequestModel>(request);
        return vm;
    }

    public async Task<bool> ValidateEdit(string projectname, int id, CreateUpdateRequestModel request, ModelStateDictionary modelState)
    {
        return modelState.IsValid;
    }

    public async Task Edit(string projectName, int id, CreateUpdateRequestModel request)
    {
        var user = contextAccessor.HttpContext.User.Parse<ApplicationUser>();
        Guard.Against.Null(user, nameof(ApplicationUser));

        var existing = await _requestRepository.Get(user.Id, projectName, id);
        if (existing != null)
        {
            var mapped = _mapper.Map<Core.Entities.Requests.Request>(request);
            await _requestRepository.Update(user.Id, projectName, id, mapped);
        }
    }
}
