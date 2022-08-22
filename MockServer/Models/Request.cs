namespace MockServer.Models;

public class Request
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    //public Dictionary<string, string> Headers { get; set; }
    public Response Response { get; set; }
    public int WorkspaceId { get; set; }
    public Workspace Workspace { get; set; }
}