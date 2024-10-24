using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Identity;


namespace GigKompassen.Models.Accounts
{
  public class ApplicationUser : IdentityUser<Guid>
  {
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool ProfileCompleted { get; set; } = false;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }


    // Navigation property for chat participants
    public List<Profile>? OwnedProfiles { get; set; } = new List<Profile>();
    public List<ChatParticipant>? ChatParticipations { get; set; } = new List<ChatParticipant>();
    public List<MediaLink>? UploadedMedia { get; set; } = new List<MediaLink>();
    public List<ProfileAccess>? ProfilesAccesses { get; set; } = new List<ProfileAccess>();
  }
}
