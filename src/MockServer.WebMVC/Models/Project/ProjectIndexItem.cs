using MockServer.Core.Enums;

namespace MockServer.WebMVC.Models.Project;

public class ProjectIndexItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
}