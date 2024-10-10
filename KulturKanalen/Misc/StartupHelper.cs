using KulturKanalen.Enums;
using KulturKanalen.Models.Accounts;

using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KulturKanalen.Misc
{
  public static class StartupHelper
  {
    public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
    {
      foreach (var roleName in Enum.GetNames(typeof(ApplicationRoleTypes)))
      {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
          await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }
      }
    }
  }
}
