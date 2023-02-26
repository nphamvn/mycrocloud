namespace MockServer.Core.Identity;
public class User: BaseEntity
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
}