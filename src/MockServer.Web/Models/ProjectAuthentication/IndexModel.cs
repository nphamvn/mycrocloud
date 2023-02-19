using MockServer.Core.Models.Projects;

namespace MockServer.Web.Models.ProjectAuthentication;

public class IndexViewModel
{
    public Project Project { get; set; }
    public IEnumerable<AuthenticationScheme> AuthenticationSchemes { get; set; }
}
