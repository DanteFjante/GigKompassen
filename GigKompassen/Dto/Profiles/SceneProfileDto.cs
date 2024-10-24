using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

namespace GigKompassen.Dto.Profiles
{
  public class SceneProfileDto
  {
    public Guid? Id { get; set; }
    public string? VenueName { get; set; }
    public string? Address { get; set; }
    public string? VenueType { get; set; }
    public string? ContactInfo { get; set; }
    public int? Capacity { get; set; }
    public string? Bio { get; set; }
    public string? Description { get; set; }
    public string? Amenities { get; set; }
    public string? OpeningHours { get; set; }

    public List<GenreDto>? Genres { get; set; }

  }
}