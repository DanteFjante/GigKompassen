using GigKompassen.Enums;
using GigKompassen.Models.Chat;

using Microsoft.AspNetCore.Identity;

using System.ComponentModel.DataAnnotations;


namespace GigKompassen.Models.Accounts
{
  public class ApplicationUser : IdentityUser<Guid>
  {
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastLoggedIn { get; set; }

    public bool ProfileCompleted { get; set; } = false;

    // Navigation property for chat participants
    public ICollection<ChatParticipant> ChatParticipants { get; set; }
  }
}
