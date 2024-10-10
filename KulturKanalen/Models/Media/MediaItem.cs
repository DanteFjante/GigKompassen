using GigKompassen.Enums;
using GigKompassen.Models.Accounts;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Models.Media
{

  public class MediaItem
  {
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid? ApplicationUserId { get; set; } // Uploader
    public ApplicationUser ApplicationUser { get; set; }

    [Required]
    public MediaType MediaType { get; set; }

    public DateTime UploadDate { get; set; } = DateTime.UtcNow;

    [Required]
    public string FilePath { get; set; } // URL or path

    public string Title { get; set; }

    public string Description { get; set; }

    // Foreign key to MediaGallery
    public Guid? MediaGalleryId { get; set; }

    public MediaGallery MediaGallery { get; set; }
  }
}