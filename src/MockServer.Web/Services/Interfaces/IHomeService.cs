using MockServer.Web.Models.Home;

namespace MockServer.Web.Services.Interfaces;

public interface IHomeService
{
    Task<IEnumerable<ServiceItem>> GetAllServices();
}
