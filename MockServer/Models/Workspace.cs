namespace MockServer.Models;

public class Workspace
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public ICollection<Request> Requests { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
}