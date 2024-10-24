using GigKompassen.Authorization.Identity;
using GigKompassen.Data;
using GigKompassen.Models.Accounts;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;

namespace GigKompassen.Services
{
  public class IdentityProfileService
  {

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public IdentityProfileService(UserManager<ApplicationUser> userManager, ApplicationDbContext context) 
    {
      _userManager = userManager;
      _context = context;
    }

    public async Task<List<ProfileIdentity?>> GetProfilesFromUserAsync(ApplicationUser user)
    {
      return (await _userManager.GetClaimsAsync(user)).Select(c => ProfileClaim.ParseProfileClaimn(c)).Where(pi => pi.HasValue).ToList();
    }

    public async Task<List<ApplicationUser>> GetUsersFromProfileAsync(Guid profileId)
    {
      List<IdentityUserClaim<Guid>> identityUserClaims = await _context.UserClaims
        .AsNoTracking()
        .Where(uc => uc.ClaimValue != null && uc.ClaimValue.Equals(profileId.ToString()) && uc.ClaimType != null)
        .ToListAsync();
      var users = await Task.WhenAll(identityUserClaims.Select(async uc => await _userManager.FindByIdAsync(uc.UserId.ToString())));

      return users.Where(au => au != null).Select(au => au!).ToList();
    }

    public async Task<bool> AddUserToProfileAsync(ApplicationUser newUser, ProfileIdentity profileIdentity)
    {
      var result = await _userManager.AddClaimAsync(newUser, new ProfileClaim(profileIdentity));
      return result.Succeeded;
    }

    public async Task<bool> AddUserToProfileAsync(ClaimsPrincipal user, ProfileIdentity profileIdentity)
    {
      var appuser = await _userManager.GetUserAsync(user);
      var result = await _userManager.AddClaimAsync(appuser, new ProfileClaim(profileIdentity));
      return result.Succeeded;
    }

    public async Task DeleteAllUsersOfProfileAsync(ProfileTypeEnum profiletype, Guid profileId)
    {
      List<IdentityUserClaim<Guid>> identityUserClaims = await _context.UserClaims
        .AsNoTracking()
        .Where(uc => uc.ClaimValue != null && uc.ClaimValue.Equals(profileId.ToString()) && uc.ClaimType != null)
        .ToListAsync();

      foreach (var claim in identityUserClaims)
      {
        ProfileIdentity? profileIdentity = ProfileClaim.ParseProfileClaimn(claim.ToClaim());
        ApplicationUser? user = await _userManager.FindByIdAsync(claim.UserId.ToString());
        if (profileIdentity.HasValue && profileIdentity.Value.ProfileType == profiletype && user != null)
        {
          await _userManager.RemoveClaimAsync(user, new ProfileClaim(profileIdentity.Value));
        }
      }
    }

    public async Task<bool> DeleteUserFromProfileAsync(ApplicationUser user, Guid profileId)
    {
      var claims = (await _userManager.GetClaimsAsync(user)).Where(c => c.Value.Equals(profileId.ToString()));
      var result = await _userManager.RemoveClaimsAsync(user, claims);
      return result.Succeeded;
    }

    public async Task<bool> DeleteUserFromProfileAsync(ClaimsPrincipal user, Guid profileId)
    {
      var appuser = await _userManager.GetUserAsync(user);
      var claims = (await _userManager.GetClaimsAsync(appuser)).Where(c => c.Value.Equals(profileId));
      var result = await _userManager.RemoveClaimsAsync(appuser, claims);
      return result.Succeeded;
    }


  }
}
