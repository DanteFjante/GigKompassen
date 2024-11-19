using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;

namespace GigKompassen.Services
{
  public class ProfileAccessService
  {
    private readonly ApplicationDbContext _context;

    public event AsyncEventHandler<ProfileAccess> OnAddAuthorization;
    public event AsyncEventHandler<ProfileAccess> OnRemoveAuthorization;
    public event AsyncEventHandler<List<ProfileAccess>> OnRemoveAuthorizations;
    public event AsyncEventHandler<BaseProfile> OnSetProfileOwner;

    public ProfileAccessService(ApplicationDbContext context) 
    {
      _context = context;

    }

    public async Task<bool> CanAccessProfileAsync(Guid userId, Guid profileId, AccessType accessType)
    {
      var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);
      if(user == null)
        throw new KeyNotFoundException("User not found");

      var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
      if (profile == null)
        throw new KeyNotFoundException("Profile not found");

      if(profile.Owner?.Id == userId)
        return true;

      if(accessType == AccessType.View && profile.Public)
      {
        return true;
      }

      List<ProfileAccess> accessList = await _context.ProfileAccesses.Where(p => p.UserId == userId && p.ProfileId == profileId).ToListAsync();

      return accessList.Any(p => p.AccessType == accessType);
    }

    public async Task<ProfileAccess> AddProfileAuthorizationAsync(Guid userId, Guid profileId, AccessType accessType)
    {
      var user = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      var profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
      if (profile == null)
        throw new KeyNotFoundException("Profile not found");

      var profileAccess = new ProfileAccess()
      {
        Id = Guid.NewGuid(),
        AccessType = accessType,
        ProfileId = profileId,
        Profile = profile,
        UserId = userId,
        User = user
      };

      if(OnAddAuthorization != null)
        await OnAddAuthorization.InvokeAsync(this, profileAccess);

      _context.ProfileAccesses.Add(profileAccess);

      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add ProfileAccess");

      return profileAccess;
    }

    public async Task<bool> RemoveAuthorizationAsync(Guid userId, Guid profileId)
    {
      if (!_context.Users.Any(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      if (!_context.Profiles.Any(p => p.Id == profileId))
        throw new KeyNotFoundException("Profile not found");

      var auths = await _context.ProfileAccesses.Where(p => p.UserId == userId && p.ProfileId == profileId).ToListAsync();
      if(auths == null || !auths.Any())
      {
        return false;
      }

      if(OnRemoveAuthorization != null)
        await OnRemoveAuthorizations.InvokeAsync(this, auths);

      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAuthorizationAsync(Guid userId, Guid profileId, AccessType accessType)
    {
      if (!_context.Users.Any(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      if (!_context.Profiles.Any(p => p.Id == profileId))
        throw new KeyNotFoundException("Profile not found");

      var auths = await _context.ProfileAccesses.Where(p => p.ProfileId == profileId && p.UserId == userId && p.AccessType == accessType).ToListAsync();
      if (auths == null || !auths.Any())
      {
        return false;
      }

      if(OnRemoveAuthorizations != null)
        await OnRemoveAuthorizations.InvokeAsync(this, auths);

      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAuthorizationAsync(Guid profileAccessId)
    {
      var profileAccess = await _context.ProfileAccesses.FirstOrDefaultAsync(p => p.Id == profileAccessId);
      if (profileAccess == null)
      {
        return false;
      }

      if (OnRemoveAuthorization != null)
        await OnRemoveAuthorization.InvokeAsync(this, profileAccess);

      _context.ProfileAccesses.Remove(profileAccess);
      int result = await _context.SaveChangesAsync();
      return result == 1;
    }

    public async Task<bool> ClearAuthorizationsFromProfileAsync(Guid profileId)
    {

      var auths = await _context.ProfileAccesses.Where(p => p.ProfileId == profileId).ToListAsync();
      if (auths == null || !auths.Any())
      {
        return false;
      }

      if(OnRemoveAuthorizations != null)
        await OnRemoveAuthorizations.InvokeAsync(this, auths);

      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ClearAuthorizationsFromUserAsync(Guid userId)
    {
      var auths = await _context.ProfileAccesses.Where(p => p.UserId == userId).ToListAsync();
      if (auths == null || !auths.Any())
      {
        return false;
      }
      if(OnRemoveAuthorizations != null)
        await OnRemoveAuthorizations.InvokeAsync(this, auths);

      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SetProfileOwnerAsync(Guid userId, Guid profileId)
    {
      ApplicationUser? user = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);
      if(user == null)
        throw new KeyNotFoundException("User not found");

      BaseProfile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
      if (profile == null)
        throw new KeyNotFoundException("Profile not found");

      profile.Owner = user;

      if(OnSetProfileOwner != null)
        await OnSetProfileOwner.InvokeAsync(this, profile);

      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to set Profile Owner");

      return true;
    }

  }
}
