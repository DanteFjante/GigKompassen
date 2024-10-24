using GigKompassen.Enums;
using GigKompassen.Models.Media;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class SceneProfile : Profile
  {
    public string Address { get; set; } = String.Empty;
    public string VenueType { get; set; } = String.Empty;
    public string ContactInfo { get; set; } = String.Empty;
    public int Capacity { get; set; } = 0;
    public string Bio { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public string Amenities { get; set; } = String.Empty;
    public string OpeningHours { get; set; } = String.Empty;

    // Navigation properties
    public MediaGalleryOwner? GalleryOwner { get; set; }
    public List<Genre>? Genres { get; set; } = new List<Genre>();

    public override ProfileTypes ProfileType => ProfileTypes.Scene;
  }
}
