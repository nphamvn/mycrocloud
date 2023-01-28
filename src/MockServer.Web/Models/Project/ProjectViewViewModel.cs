using MockServer.Web.Models.Request;

namespace MockServer.Web.Models.Project;

public class ProjectViewViewModel
{
    public ProjectIndexItem ProjectInfo { get; set; }
    public IEnumerable<RequestItem> Requests { get; set; }
}
