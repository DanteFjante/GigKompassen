using KulturKanalen.Enums;
using KulturKanalen.Models.Chat;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;


namespace KulturKanalen.Models.Accounts
{
  public class ApplicationUser : IdentityUser<Guid>
  {
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastLoggedIn { get; set; }

    [Required]
    public ApplicationRoleTypes ApplicationRole { get; set; }


    // Navigation property for chat participants
    public ICollection<ChatParticipant> ChatParticipants { get; set; }
  }
}
