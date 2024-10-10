using KulturKanalen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace KulturKanalen.Models.Scenes
{
  public class SceneProfile
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string VenueName { get; set; }

    public string Address { get; set; }

    public string VenueType { get; set; }

    public string ContactInfo { get; set; }

    public int Capacity { get; set; }

    public string Amenities { get; set; }

    public string Description { get; set; }

    public string OpeningHours { get; set; }

    // Foreign key to MediaGallery
    public Guid? MediaGalleryId { get; set; }

    public MediaGallery MediaGallery { get; set; }

    // Foreign key to ManagementProfile
    [Required]
    public Guid ManagementProfileId { get; set; }

    public ManagementProfile ManagementProfile { get; set; }
  }
}
