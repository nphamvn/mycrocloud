using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockServer.WebMVC.Models.Request;

namespace MockServer.WebMVC.Services.Interfaces;

public interface IRequestService
{
    Task<int> Create(string projectName, CreateRequestViewModel request);
    Task<RequestItem> Get(string projectname, int id);
    Task<FixedRequestConfigViewModel> GetFixedRequestConfigViewModel(string projectname, int id);
    Task SaveFixedRequestConfig(string projectname, int id, string[] fields, FixedRequestConfigViewModel config);
    Task Delete(string projectname, int id);
}
