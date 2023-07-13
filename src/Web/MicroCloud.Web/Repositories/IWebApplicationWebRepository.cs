using MicroCloud.Web.Models.WebApplications;
using IBaseWebApplicationRepository = WebApplication.Domain.Repositories.IWebApplicationRepository;
namespace MicroCloud.Web.Repositories;
public interface IWebApplicationWebRepository : IBaseWebApplicationRepository
{
    Task<IEnumerable<WebApplicationIndexItem>> GetWebApplicationIndexItems(string userId);
}