using MockServer.Core.Enums;
using MockServer.Web.Models.Projects;

namespace MockServer.Web.Models.ProjectRequests;

public class Request
{
    public int Id { get; set; }
    public string Name { get; set; }
    public RequestType Type { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public string Description { get; set; }
    public Project Project { get; set; }
}
