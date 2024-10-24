using GigKompassen.Authorization.Policies.Requirements;
using GigKompassen.Models.Accounts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Authorization.Policies.Handlers
{
  public class ProfileComleteHandler : AuthorizationHandler<ProfileCompleteRequirement>
  {

    private readonly UserManager<ApplicationUser> _userManager;

    public ProfileComleteHandler(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileCompleteRequirement requirement)
    {
      if (context.User.Identity.IsAuthenticated)
      {
        var user = await _userManager.GetUserAsync(context.User);
        if (user != null && user.ProfileCompleted)
        {
          context.Succeed(requirement);
        }
        else
        {
          context.Fail();
        }
      }
    }
  }
}
