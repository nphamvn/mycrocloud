using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Services.Interfaces;

public interface IRequestWebService
{
    Task<int> Create(string projectName, CreateUpdateRequestViewModel request);
    Task<bool> ValidateEdit(string projectname, int id, CreateUpdateRequestViewModel request, ModelStateDictionary modelState);
    Task Edit(string projectname, int id, CreateUpdateRequestViewModel request);
    Task<CreateUpdateRequestViewModel> GetCreateRequestViewModel(string projectName);
    Task<CreateUpdateRequestViewModel> GetRequestModel(string projectName, int requestId);
    Task<RequestOpenViewModel> GetRequestOpenViewModel(string projectName, int requestId);
    Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config);
    Task Delete(string projectname, int id);
}
