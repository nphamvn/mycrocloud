using MockServer.Core.Enums;

namespace MockServer.Core.Models.Services;

public class Service: BaseEntity
{
    public ServiceType Type { get; set; }
    public string Name { get; set; }   
}
