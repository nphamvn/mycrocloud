using MockServer.Web.Models.Home;
using MockServer.Web.Services.Interfaces;

namespace MockServer.Web.Services;

public class HomeService : IServiceWebService
{
    public Task<IEnumerable<ServiceItem>> GetAllServices()
    {
        throw new NotImplementedException();
    }
}
