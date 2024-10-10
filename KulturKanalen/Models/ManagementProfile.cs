using KulturKanalen.Models.Accounts;
using KulturKanalen.Models.Artists;
using KulturKanalen.Models.Scenes;

using System.ComponentModel.DataAnnotations;

namespace KulturKanalen.Models
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