namespace MockServer.DTOs;

public class RequestDto
{
    public string Username { get; set; }
    public string Workspace { get; set; }
    public string Path { get; set; }
    public string Method { get; set; }
    public IDictionary<string, string> Queries { get; set; }
}