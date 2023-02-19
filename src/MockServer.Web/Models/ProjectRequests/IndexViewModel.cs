using MockServer.Web.Models.Projects;

namespace MockServer.Web.Models.ProjectRequests;

public class IndexViewModel
{
    public Project Project { get; set; }
    public IEnumerable<RequestIndexItem> Requests { get; set; }
}
