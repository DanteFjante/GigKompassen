using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Media
{

  public class MediaItem
  {
    public Guid Id { get; set; }
    public string Title { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;

    // Foreign keys
    public Guid GalleryId { get; set; }
    public Guid? MediaLinkId { get; set; }

    // Navigation properties
    public MediaGallery? Gallery { get; set; }
    public MediaLink? MediaLink { get; set; }
  }
}