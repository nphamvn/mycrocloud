namespace Api.Entities;

public class UserContact
{
    public string UserId { get; set; }

    public User User { get; set; }
    
    public string ContactId { get; set; }
    
    public User Contact { get; set; }

    public DateTime CreatedAt { get; set; }
}