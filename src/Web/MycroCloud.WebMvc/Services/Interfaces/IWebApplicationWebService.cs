using MicroCloud.Web.Models.WebApplications;

namespace MicroCloud.Web.Services;

public interface IWebApplicationService
{
    Task<WebAppModel> Get(int appId);
    Task<WebAppIndexViewModel> GetIndexViewModel(WebAppSearchModel searchModel);
    Task Create(WebAppCreateViewModel app);
    Task Rename(int appId, string name);
    Task Delete(int appId);
}