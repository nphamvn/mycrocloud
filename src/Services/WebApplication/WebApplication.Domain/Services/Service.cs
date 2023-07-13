using WebApplication.Domain.Identity;

namespace WebApplication.Domain.Services.Entities;

public class Service: BaseEntity
{
    public string UserId { get; set; }
    public User User { get; set; }
    public ServiceType Type { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; } 
}
