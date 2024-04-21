namespace Api.Entities;

public class ConversationUser
{
    public string UserId { get; set; }
    
    public int ConversationId { get; set; }

    public Conversation Conversation { get; set; }
    
    public User User { get; set; }
}