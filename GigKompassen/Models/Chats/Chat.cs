using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Chats
{
  public class Chat
  {
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public DateTime Created { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<ChatMessage>? Messages { get; set; } = new List<ChatMessage>();
    public List<ChatParticipant>? Participants { get; set; } = new List<ChatParticipant>();
    public MediaGalleryOwner? GalleryOwner { get; set; }

  }
}
