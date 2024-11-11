namespace GigKompassen.Models.Media
{
  public class MediaGalleryOwner
  {
    public Guid Id { get; set; }


    // Navigation properties
    public List<MediaGallery>? Galleries { get; set; }
  }
}
