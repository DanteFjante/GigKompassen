using KulturKanalen.Enums;
using KulturKanalen.Models.Accounts;
using KulturKanalen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace KulturKanalen.Models.Chat
{
  public class ChatMessageContent
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public ChatMessageType ChatMessageType { get; set; }

    [Required]
    public DateTime DateTimeCreated { get; set; } = DateTime.UtcNow;

    public string Message { get; set; }

    public Guid? MediaItemId { get; set; }
    public MediaItem MediaItem { get; set; }

    public bool FlaggedForDeletion { get; set; } = false;

    // Audit fields
    [Required]
    public Guid CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public Guid? ModifiedByUserId { get; set; }
    public ApplicationUser ModifiedByUser { get; set; }

    // Optional: Navigation property to ChatMessages
    public ICollection<ChatMessage> ChatMessages { get; set; }
  }
}
