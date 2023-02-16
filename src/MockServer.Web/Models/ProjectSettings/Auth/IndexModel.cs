using MockServer.Core.Models.Auth;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Models.ProjectSettings.Auth;

public class AuthIndexModel
{
    public Project Project { get; set; }
    public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
}
