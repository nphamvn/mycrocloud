using MockServer.Core.WebApplications;
using MockServer.Web.Models.WebApplications;
using WebApplication = MockServer.Web.Models.WebApplications.WebApplication;
namespace MockServer.Web.Services;

public interface IWebApplicationWebService
{
    Task<WebApplication> Get(int appId);
    Task<WebApplicationIndexViewModel> GetIndexViewModel(WebApplicationSearchModel searchModel);
    Task Create(WebApplicationCreateModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
    Task SetAccessibility(int appId, WebApplicationAccessibility accessibility);
    Task<WebApplicationViewModel> GetOverviewViewModel(int webApplicationId);
}