namespace MockServer.Core.Entities;

public class AppUser : BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public ICollection<Project> Projects { get; set; }
}