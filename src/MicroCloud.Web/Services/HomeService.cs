using MicroCloud.Web.Models.Home;
using MicroCloud.Web.Services.Interfaces;

namespace MicroCloud.Web.Services;

public class HomeService : IServiceWebService
{
    public Task<IEnumerable<ServiceItem>> GetAllServices()
    {
        throw new NotImplementedException();
    }
}
