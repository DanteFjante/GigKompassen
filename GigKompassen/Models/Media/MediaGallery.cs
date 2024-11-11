using GigKompassen.Models.Chats;

namespace GigKompassen.Models.Media
{
  public class MediaGallery
  {
    public Guid Id { get; set; }
    public string Name { get; set; }

    //Foreign Keys
    public Guid OwnerId { get; set; }

    // Navigation property
    public MediaGalleryOwner? Owner { get; set; }
    public List<MediaItem> Items { get; set; } = new List<MediaItem>();
  }
}