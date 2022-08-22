namespace MockServer.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public ICollection<Workspace> Workspaces { get; set; }
}