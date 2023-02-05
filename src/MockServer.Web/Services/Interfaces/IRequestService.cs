using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Models.Requests;

namespace MockServer.Web.Services.Interfaces;

public interface IRequestWebService
{
    Task<int> Create(int projectId, CreateUpdateRequestViewModel request);
    Task<bool> ValidateEdit(int id, CreateUpdateRequestViewModel request, ModelStateDictionary modelState);
    Task Edit(int id, CreateUpdateRequestViewModel request);
    Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel();
    Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel(int requestId);
    Task<AuthorizationConfigViewModel> GetAuthorizationConfigViewModel(int projectId, int requestId);
    Task ConfigureRequestAuthorization(int requestId, AuthorizationConfigViewModel auth);
    Task<RequestOpenViewModel> GetRequestOpenViewModel(int requestId);
    Task SaveFixedRequestConfig(int id, string[] fields, FixedRequestConfigViewModel config);
    Task Delete(int id);
}
