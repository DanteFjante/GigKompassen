using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class ArtistMember
  {
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
    [Required]
    public required string Role { get; set; }

    // Foreign key
    [Required]
    public Guid ArtistProfileId { get; set; }

    // Navigation property
    public ArtistProfile? ArtistProfile { get; set; }
  }
}
