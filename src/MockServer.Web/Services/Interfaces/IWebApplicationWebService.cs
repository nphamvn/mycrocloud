using MockServer.Web.Models.WebApplications;
using WebApplication = MockServer.Web.Models.WebApplications.WebApplication;
namespace MockServer.Web.Services;

public interface IWebApplicationService
{
    Task<WebApplication> Get(int appId);
    Task<WebApplicationIndexViewModel> GetIndexViewModel(WebApplicationSearchModel searchModel);
    Task Create(WebApplicationCreateModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}