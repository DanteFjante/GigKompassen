using GigKompassen.Enums;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class ManagerProfile : Profile
  {
    public string Description { get; set; } = String.Empty;
    public string Location { get; set; } = String.Empty;

    public override ProfileTypes ProfileType => ProfileTypes.Manager;
  }
}