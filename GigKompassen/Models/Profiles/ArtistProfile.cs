using GigKompassen.Enums;
using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class ArtistProfile : Profile
  {
    public string Location { get; set; } = String.Empty;
    public string Bio { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;

    public AvailabilityStatus Availability { get; set; } = AvailabilityStatus.Open;

    // Navigation properties
    public MediaGalleryOwner? GalleryOwner { get; set; }
    public List<ArtistMember>? Members { get; set; } = new List<ArtistMember>();
    public List<Genre>? Genres { get; set; } = new List<Genre>();

    public override ProfileTypes ProfileType => ProfileTypes.Artist;
  }
}
