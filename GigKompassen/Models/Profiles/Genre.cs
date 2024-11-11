using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Profiles
{
  public class Genre
  {
    [Key]
    public Guid Id { get; set; }
    [Required]
    public required string Name { get; set; }
  }
}
