using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Models.Requests;

namespace MockServer.Core.Models.Projects;

public class Project : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ApplicationUser User { get; set; }
    public ICollection<Request> Requests { get; set; }
    public List<AuthenticationScheme> Authentications { get; set; }
    public List<string> UseMiddlewares { get; set; }
    public bool Blocked { get; set; }
}