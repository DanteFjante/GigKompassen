using GigKompassen.Enums;
using GigKompassen.Models.Profiles;

namespace GigKompassen.Models.Accounts
{
  public class ProfileAccess
  {
    public Guid Id { get; set; }
    public AccessType AccessType { get; set; }

    // Foreign keys
    public Guid UserId { get; set; }
    public Guid ProfileId { get; set; }

    // Navigation properties
    public ApplicationUser? User { get; set; }
    public BaseProfile? Profile { get; set; }
  }
}
