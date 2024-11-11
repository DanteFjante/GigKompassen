using GigKompassen.Enums;

namespace GigKompassen.Models.Profiles
{
  public class ManagerProfile : BaseProfile
  {
    public string Description { get; set; } = String.Empty;
    public string Location { get; set; } = String.Empty;

    public override ProfileTypes ProfileType => ProfileTypes.Manager;
  }
}