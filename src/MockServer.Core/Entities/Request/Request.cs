using MockServer.Core.Enums;
using MockServer.Core.Entities.Projects;
namespace MockServer.Core.Entities.Requests;
public class Request : BaseEntity
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public RequestType Type { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public string Description { get; set; }
    public string Method { get; set; }
    public Project Project { get; set; }
    public int Authorization { get; set; }
}

