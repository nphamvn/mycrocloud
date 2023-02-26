using MockServer.Web.Models.Home;

namespace MockServer.Web.Services;

public interface IServiceWebService
{
    Task<IEnumerable<ServiceItem>> GetAllServices();
}
