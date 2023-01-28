using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.Web.Models.Project;
using MockServer.Web.Models.Request;

namespace MockServer.Web.Services.Interfaces;

public interface IRequestWebService
{
    Task<int> Create(string projectName, CreateUpdateRequestViewModel request);
    Task<bool> ValidateEdit(string projectname, int id, CreateUpdateRequestViewModel request, ModelStateDictionary modelState);
    Task Edit(string projectname, int id, CreateUpdateRequestViewModel request);
    Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel(string projectName);
    Task<CreateUpdateRequestViewModel> GetGetCreateRequestViewModel(string projectName, int requestId);
    Task<AuthorizationConfigViewModel> GetAuthorizationConfigViewModel(string projectName, int requestId);
    Task UpdateRequestAuthorizationConfig(string projectName, int requestId, AuthorizationConfigViewModel auth);
    Task<RequestOpenViewModel> GetRequestOpenViewModel(string projectName, int requestId);
    Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config);
    Task Delete(string projectname, int id);
}
