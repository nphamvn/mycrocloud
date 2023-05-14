namespace MockServer.Core.Identity;
public class User: BaseEntity
{
    public new string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string AvatarUrl { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
}