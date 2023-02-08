namespace MockServer.Core.UserPool.Models;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public UserProfile Profile { get; set; }
}
