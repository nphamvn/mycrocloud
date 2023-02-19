using MockServer.Core.Enums;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Models.ProjectAuthentication;

public abstract class AuthenticationScheme
{
    public int Id { get; set; }
    public abstract AuthenticationSchemeType Type { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public int Order { get; set; }
    public string Description { get; set; }
    public Project Project { get; set; }
}
