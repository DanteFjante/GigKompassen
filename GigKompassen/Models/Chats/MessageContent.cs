using GigKompassen.Enums;
using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Chats
{
  public abstract class MessageContent
  {
    public Guid Id { get; set; }
    public DateTime Created { get; set; }
    public virtual ChatMessageType DataType { get; } // Enum(Text, Media)

    // No foreign key to ChatMessage to avoid cycles
  }

  public class MessageTextContent : MessageContent
  {
    [Required]
    public required string Text { get; set; }
    public override ChatMessageType DataType => ChatMessageType.Text;
  }

  public class MessageMediaContent : MessageContent
  {
    public string? Comment { get; set; }
    public override ChatMessageType DataType => ChatMessageType.Media;

    // Foreign key
    public Guid? MediaLinkId { get; set; }

    // Navigation property
    public MediaLink? MediaLink { get; set; }

  }
}
