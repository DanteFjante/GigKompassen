using GigKompassen.Models.Accounts;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GigKompassen.Models.Chat
{
  public class ChatParticipant
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ChatId { get; set; }
    public Chat Chat { get; set; }

    [Required]
    public Guid ApplicationUserId { get; set; }
    public ApplicationUser ApplicationUser { get; set; }

    public Guid? LastReadMessageId { get; set; }
    public ChatMessage LastReadMessage { get; set; }
  }
}
