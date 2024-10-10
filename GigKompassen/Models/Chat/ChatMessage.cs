using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Chat
{
  public class ChatMessage
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ChatId { get; set; }

    public Chat Chat { get; set; }

    [Required]
    public DateTime DateTimeSent { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid ChatParticipantId { get; set; }

    public ChatParticipant ChatParticipant { get; set; } // Sender

    public Guid? ReplyToId { get; set; }

    public ChatMessage ReplyTo { get; set; }

    [Required]
    public Guid ChatMessageContentId { get; set; }

    public ChatMessageContent ChatMessageContent { get; set; }

  }
}
