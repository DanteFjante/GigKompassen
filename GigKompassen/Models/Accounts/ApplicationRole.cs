using Microsoft.AspNetCore.Identity;

namespace GigKompassen.Models.Accounts
{
  public class ApplicationRole : IdentityRole<Guid>
  {
    public ApplicationRole() { }
    public ApplicationRole(string roleName) : base(roleName) { }
  }
}
