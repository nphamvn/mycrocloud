namespace MockServer.Models.Responses.Workspaces;

public class GetWorkspaceRequestModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public GetWorkspaceRequestResponseModel Response { get; set; }
}
public class GetWorkspaceRequestResponseModel
{
    public int StatusCode { get; set; }
    public string Body { get; set; }
}