using MicroCloud.Web.Models.Home;

namespace MicroCloud.Web.Services;

public interface IServiceWebService
{
    Task<IEnumerable<ServiceItem>> GetAllServices();
}
