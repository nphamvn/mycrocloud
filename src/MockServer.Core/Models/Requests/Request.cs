using MockServer.Core.Enums;
using MockServer.Core.Models.Auth;
using MockServer.Core.Models.Projects;

namespace MockServer.Core.Models.Requests;
public class Request : BaseEntity
{
    public int ProjectId { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string Method { get; set; }
    public Project Project { get; set; }
    public Authorization Authorization { get; set; }
    public IList<RequestHeader> Headers { get; set; }
    public IList<RequestQuery> Query { get; set; }
}

