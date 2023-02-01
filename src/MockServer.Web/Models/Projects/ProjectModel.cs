using MockServer.Core.Enums;

namespace MockServer.Web.Models.Projects;

public class Project
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}