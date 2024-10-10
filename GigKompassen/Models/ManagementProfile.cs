using GigKompassen.Models.Accounts;
using GigKompassen.Models.Artists;
using GigKompassen.Models.Scenes;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models
{
  public class ManagementProfile
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ApplicationUserId { get; set; }

    public ApplicationUser ApplicationUser { get; set; }

    public ICollection<ArtistProfile> ArtistProfiles { get; set; }

    public ICollection<SceneProfile> SceneProfiles { get; set; }
  }
}