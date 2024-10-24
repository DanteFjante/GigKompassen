using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Dto.Profiles
{
  public class GenreDto
  {
    public GenreDto()
    {
    }

    public GenreDto(Guid? id, string name)
    {
      Id = id;
      Name = name;
    }

    public GenreDto(string name)
    {
      Name = name;
    }

    public Guid? Id { get; set; }
    public string Name { get; set; }
  }
}
