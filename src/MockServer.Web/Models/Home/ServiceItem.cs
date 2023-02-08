using MockServer.Core.Enums;

namespace MockServer.Web.Models.Home;

public class ServiceItem
{
    public ServiceType ServiceType { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
}
