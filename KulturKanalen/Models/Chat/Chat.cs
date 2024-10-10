using KulturKanalen.Models.Media;
using System.ComponentModel.DataAnnotations;

namespace KulturKanalen.Models.Chat
{
  public class Chat
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public string ChatName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public ICollection<ChatParticipant> Participants { get; set; }

    public ICollection<ChatMessage> Messages { get; set; }

    // Optional media gallery for chat
    public Guid? MediaGalleryId { get; set; }

    public MediaGallery MediaGallery { get; set; }
  }
}
