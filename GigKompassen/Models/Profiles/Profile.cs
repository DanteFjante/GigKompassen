using GigKompassen.Enums;
using GigKompassen.Models.Accounts;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Models.Profiles
{
  public abstract class Profile
  {
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }

    [Required]
    public bool Public { get; set; } = true;

    [Required]
    public virtual ProfileTypes ProfileType { get; }

    public List<ProfileAccess>? ProfileAccesses { get; set; }

    public Guid OwnerId { get; set; }
    public ApplicationUser? Owner { get; set; }
  }
}
