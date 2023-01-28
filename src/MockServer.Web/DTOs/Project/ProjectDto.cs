using MockServer.Core.Enums;

namespace MockServer.Web.DTOs.Project;

public class ProjectDto
{
    public string Name { get; set; }
    public ProjectAccessibility Accessibility { get; set; }
    public string Description { get; set; }
    public int RequestCount { get; set; }
}