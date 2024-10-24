using GigKompassen.Models.Chats;
using GigKompassen.Models.Profiles;

namespace GigKompassen.Models.Media
{
  public class MediaGalleryOwner
  {
    public Guid Id { get; set; }

    // Foreign keys
    public Guid? ChatId { get; set; }
    public Guid? ArtistProfileId { get; set; }
    public Guid? SceneProfileId { get; set; }

    // Navigation properties
    public MediaGallery? Gallery { get; set; }
    public Chat? Chat { get; set; }
    public ArtistProfile? ArtistProfile { get; set; }
    public SceneProfile? SceneProfile { get; set; }
  }
}
