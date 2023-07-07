using MockServer.Web.Models.WebApplications;
using IBaseWebApplicationRepository = MockServer.Domain.Repositories.IWebApplicationRepository;
namespace MockServer.Web.Repositories;
public interface IWebApplicationWebRepository : IBaseWebApplicationRepository
{
    Task<IEnumerable<WebApplicationIndexItem>> GetWebApplicationIndexItems(string userId);
}