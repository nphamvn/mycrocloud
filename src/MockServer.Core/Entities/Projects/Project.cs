using MockServer.Core.Enums;
using MockServer.Core.Entities.Requests;
namespace MockServer.Core.Entities.Projects;
public class Project : BaseEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public string? PrivateKey { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public AppUser User { get; set; }
    public ICollection<Request> Requests { get; set; }
    public int Authorization { get; set; }
}

