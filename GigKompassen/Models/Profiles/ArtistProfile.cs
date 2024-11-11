using GigKompassen.Enums;

namespace GigKompassen.Models.Profiles
{
  public class ArtistProfile : BaseProfile
  {
    public string Location { get; set; } = String.Empty;
    public string Bio { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;

    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Open;

    // Navigation properties
    public List<ArtistMember>? Members { get; set; } = new List<ArtistMember>();
    public List<Genre>? Genres { get; set; } = new List<Genre>();

    public override ProfileTypes ProfileType => ProfileTypes.Artist;
  }
}
