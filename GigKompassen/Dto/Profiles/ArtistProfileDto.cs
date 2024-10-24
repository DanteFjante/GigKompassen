using GigKompassen.Enums;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Dto.Profiles
{
  public class ArtistProfileDto
  {
    public Guid? Id { get; set; }

    public string? Name { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? Description { get; set; }

    public AvailabilityStatus? Availability { get; set; }

    public List<ArtistMemberDto>? Members { get; set; } 
    public List<GenreDto>? Genres { get; set; }
  }
}
