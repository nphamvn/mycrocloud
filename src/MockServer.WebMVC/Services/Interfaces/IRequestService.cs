using Microsoft.AspNetCore.Mvc.ModelBinding;
using MockServer.WebMVC.Models.Project;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Services.Interfaces;

public interface IRequestWebService
{
    Task<int> Create(string projectName, CreateUpdateRequestModel request);
    Task<bool> ValidateEdit(string projectname, int id, CreateUpdateRequestModel request, ModelStateDictionary modelState);
    Task Edit(string projectname, int id, CreateUpdateRequestModel request);
    Task<RequestItem> Get(string projectname, int id);
    Task<CreateUpdateRequestModel> GetRequestViewModel(string projectName, int requestId);
    Task<RequestOpenViewModel> GetRequestOpenViewModel(string projectName, int requestId);
    Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(string projectname, int id);
    Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config);
    Task Delete(string projectname, int id);
}
