using MockServer.Web.Models.Requests;

namespace MockServer.Web.Models.Projects;

public class ProjectViewViewModel
{
    public ProjectIndexItem ProjectInfo { get; set; }
    public IEnumerable<RequestIndexItem> Requests { get; set; }
}
