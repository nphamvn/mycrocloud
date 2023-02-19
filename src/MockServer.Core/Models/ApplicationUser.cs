using MockServer.Core.Models.Projects;

namespace MockServer.Core.Models;
public class ApplicationUser: BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public ICollection<Project> Projects { get; set; }
}