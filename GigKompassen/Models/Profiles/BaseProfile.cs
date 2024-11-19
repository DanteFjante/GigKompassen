using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public abstract class BaseProfile
  {
    [Key]
    public Guid Id { get; set; }

    [Required]
    public required string Name { get; set; }

    [Required]
    public bool Public { get; set; } = true;

    [Required]
    public virtual ProfileTypes ProfileType { get; }

    //Foreign Keys
    public Guid OwnerId { get; set; }
    public Guid MediaGalleryOwnerId { get; set; }

    //Navigation properties
    public ApplicationUser? Owner { get; set; }
    public MediaGalleryOwner? MediaGalleryOwner { get; set; }

  }
}
