namespace MockServer.Models.Request.Workspaces;

public class AddWorkspaceRequestModel
{
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public int AccessScope { get; set; }
    public string? ApiKey { get; set; }
}