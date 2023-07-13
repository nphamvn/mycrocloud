using MicroCloud.Web.Models.WebApplications;
using Microsoft.Extensions.Options;
using WebApplication.Domain.Settings;
using BaseWebApplicationRepository = WebApplication.Infrastructure.Repositories.PostgreSql.WebApplicationRepository;

namespace MicroCloud.Web.Repositories;
public class WebApplicationWebRepository : BaseWebApplicationRepository, IWebApplicationWebRepository
{
    public WebApplicationWebRepository(IOptions<PostgresSettings> databaseOptions) : base(databaseOptions)
    {
    }

    public Task<IEnumerable<WebApplicationIndexItem>> GetWebApplicationIndexItems(string userId)
    {
        throw new NotImplementedException();
    }
}