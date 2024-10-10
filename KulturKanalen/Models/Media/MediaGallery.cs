using KulturKanalen.Models.Accounts;

using System.ComponentModel.DataAnnotations;

namespace KulturKanalen.Models.Media
{
  public class MediaGallery
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public ICollection<MediaItem> MediaItems { get; set; }
  }
}