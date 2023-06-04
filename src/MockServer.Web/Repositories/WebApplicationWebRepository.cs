using Microsoft.Extensions.Options;
using MockServer.Core.Settings;
using BaseWebApplicationRepository = MockServer.Infrastructure.Repositories.PostgreSql.WebApplicationRepository;
using MockServer.Web.Models.WebApplications;

namespace MockServer.Web.Repositories;
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