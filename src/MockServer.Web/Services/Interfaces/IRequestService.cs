using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Models.ProjectRequests;

namespace MockServer.Web.Services.Interfaces;

public interface IProjectRequestWebService
{
    Task<IndexViewModel> GetIndexViewModel(int projectId);
    Task<bool> ValidateCreate(int projectId, SaveRequestViewModel request, ModelStateDictionary modelState);
    Task<int> Create(int projectId, SaveRequestViewModel request);
    Task<bool> ValidateEdit(int requestId, SaveRequestViewModel request, ModelStateDictionary modelState);
    Task Edit(int requestId, SaveRequestViewModel request);
    Task<SaveRequestViewModel> GetEditRequestViewModel(int requestId);
    Task<SaveRequestViewModel> GetCreateRequestViewModel(int projectId);
    Task Delete(int requestId);
    Task<RequestViewModel> GetRequestOpenViewModel(int requestId);
    Task<AuthorizationConfiguration> GetAuthorization(int projectId, int requestId);
    Task AttachAuthorization(int requestId, AuthorizationConfiguration auth);
    Task SaveRequestConfiguration(int requestId, RequestConfiguration config);
}
