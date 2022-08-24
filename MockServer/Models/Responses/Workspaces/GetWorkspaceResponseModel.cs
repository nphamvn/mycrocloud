namespace MockServer.Models.Responses.Workspaces;

public class GetWorkspaceResponseModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public string AccessScope { get; set; }
    public string? ApiKey { get; set; }
}