using GigKompassen.Enums;
using GigKompassen.Models.Accounts;

using System.ComponentModel.DataAnnotations;
namespace GigKompassen.Models.Media
{
  public class MediaLink
  {
    public Guid Id { get; set; }
    public DateTime Uploaded { get; set; }
    [Required]
    public required MediaType MediaType { get; set; }
    [Required]
    public required string Path { get; set; }

    // Foreign key
    public Guid UploaderId { get; set; }

    // Navigation property
    public ApplicationUser? Uploader { get; set; }
  }
}
