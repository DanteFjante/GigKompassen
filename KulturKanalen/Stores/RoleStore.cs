using KulturKanalen.Models.Accounts;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KulturKanalen.Stores
{
  public class RoleStore : IRoleStore<ApplicationUser>
  {
    public Task<IdentityResult> CreateAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public Task<ApplicationUser?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<ApplicationUser?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string?> GetNormalizedRoleNameAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string> GetRoleIdAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<string?> GetRoleNameAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetNormalizedRoleNameAsync(ApplicationUser role, string? normalizedName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task SetRoleNameAsync(ApplicationUser role, string? roleName, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser role, CancellationToken cancellationToken)
    {
      throw new NotImplementedException();
    }
  }
}
