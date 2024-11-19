using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Identity;


namespace GigKompassen.Models.Accounts
{
  public class ApplicationUser : IdentityUser<Guid>
  {
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public bool RegistrationCompleted { get; set; } = false;
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
  }
}
