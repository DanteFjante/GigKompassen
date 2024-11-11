using GigKompassen.Models.Accounts;

namespace GigKompassen.Models.Chats
{
  public class ChatParticipant
  {
    public Guid Id { get; set; }
    public DateTime LastReadTime { get; set; }

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }

    // Navigation properties
    public Chat? Chat { get; set; }
    public ApplicationUser? User { get; set; }

    public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();

  }
}
