using WebApplication.Domain.Services;
namespace MicroCloud.Web.Models.Home;

public class ServiceItem
{
    public ServiceType ServiceType { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
}
