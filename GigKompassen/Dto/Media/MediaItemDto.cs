using GigKompassen.Models.Media;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Dto.Media
{
  public class MediaItemDto
  {
    public Guid? Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
  }
}
