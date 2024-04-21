namespace Api.Entities;

public class Conversation
{
    public int Id { get; set; }
    
    public string? Name { get; set; }

    public ICollection<ConversationUser> Users { get; set; }

    public ICollection<Message> Messages { get; set; }
    
    public DateTime CreatedAt { get; set; }
}