using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KulturKanalen.Models.Artists
{
  public class GroupMember
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ArtistProfileId { get; set; }

    public ArtistProfile ArtistProfile { get; set; }

    [Required]
    public string Name { get; set; }

    public string Role { get; set; } // Free-text
  }
}
