namespace Api.Entities;

public class Message
{
    public int Id { get; set; }

    public int ConversationId { get; set; }

    public Conversation Conversation { get; set; }
    
    public string SenderId { get; set; }
    
    public User Sender { get; set; }
    
    public string Text { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public string? ClientId { get; set; }
}