using GigKompassen.Enums;
using GigKompassen.Models.Accounts;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Dto.Media
{
  public class MediaLinkDto
  {
    public Guid? Id { get; set; }
    public DateTime? Uploaded { get; set; }
    public MediaType MediaType { get; set; }
    public string Path { get; set; }
  }
}
