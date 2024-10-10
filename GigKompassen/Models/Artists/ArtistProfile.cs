using GigKompassen.Enums;
using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Artists
{
  public class ArtistProfile
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Name { get; set; }

    public string Location { get; set; } // City

    [Required]
    public AvailabilityStatus Availability { get; set; }

    public string Bio { get; set; }

    public string Description { get; set; }

    // Foreign key to MediaGallery
    public Guid? MediaGalleryId { get; set; }

    public MediaGallery MediaGallery { get; set; }

    // Collection of group members
    public ICollection<GroupMember> GroupMembers { get; set; }

    // Foreign key to ManagementProfile
    [Required]
    public Guid ManagementProfileId { get; set; }

    public ManagementProfile ManagementProfile { get; set; }

    // Many-to-many relationship with Genre
    public ICollection<Genre> Genres { get; set; }
  }
}
