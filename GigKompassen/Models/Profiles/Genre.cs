using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class Genre
  {
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }

    // Navigation properties
    public List<ArtistProfile>? ArtistProfiles { get; set; } = new List<ArtistProfile>();
    public List<SceneProfile>? SceneProfiles { get; set; } = new List<SceneProfile>();

  }
}
