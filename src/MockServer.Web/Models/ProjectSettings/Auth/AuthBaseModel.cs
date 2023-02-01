using MockServer.Web.Models.Projects;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class AuthBaseModel
{
    public Project Project { get; set; }
    public string SchemeName { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
}
