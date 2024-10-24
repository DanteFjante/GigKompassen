using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Dto.Profiles
{
  public class ManagerProfileDto
  {
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Location { get; set; }
  }
}
