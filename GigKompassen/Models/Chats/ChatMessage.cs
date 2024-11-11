namespace GigKompassen.Models.Chats
{
  public class ChatMessage
  {
    public Guid Id { get; set; }
    public DateTime Sent { get; set; }

    // Foreign keys
    public Guid ChatId { get; set; }
    public Guid SenderId { get; set; } // ChatParticipantId
    public Guid? ContentId { get; set; }
    public Guid? ReplyToId { get; set; }

    // Navigation properties
    public Chat? Chat { get; set; }
    public ChatParticipant? Sender { get; set; }
    public MessageContent? Content { get; set; }
    public ChatMessage? ReplyTo { get; set; }

  }
}
