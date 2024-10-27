using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class ProfileAccessService
  {
    private readonly ApplicationDbContext _context;
    public ProfileAccessService(ApplicationDbContext context) 
    {
      _context = context;
    }

    public async Task<bool> CanAccessProfile(Guid profileId, Guid userId, AccessType accessType)
    {
      bool any = _context.Profiles.Any(p => p.Id == profileId);
      bool any2 = _context.Users.Any(p => p.Id == userId);
      Profile? profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profileId);
      if (profile == null)
      {
        return false;
      }

      if(accessType == AccessType.View && profile.Public)
      {
        return true;
      }

      if (profile.Owner?.Id == userId)
        return true;

      if(profile.ProfileAccesses != null && profile.ProfileAccesses.Any(p => p.UserId == userId && p.AccessType == accessType))
      {
        return true;
      }

      return false;
    }

    public async Task<bool> AddProfileAuthorization(Guid profileId, Guid userId, AccessType accessType)
    {
      var hasUser = await _context.Users.AnyAsync(p => p.Id == userId);
      var hasProfile = await _context.Profiles.AnyAsync(p => p.Id == profileId);

      if (!hasUser || !hasProfile)
      {
        return false;
      }

      var profileAccess = new ProfileAccess
      {
        ProfileId = profileId,
        UserId = userId,
        AccessType = accessType
      };

      _context.ProfileAccesses.Add(profileAccess);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> RemoveAuthorization(Profile profile, ApplicationUser user)
    {
      var auths = await _context.ProfileAccesses.Where(p => p.UserId == user.Id && p.ProfileId == profile.Id).ToListAsync();
      if(auths == null)
      {
        return false;
      }
      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAuthorization(Profile profile, ApplicationUser user, AccessType accessType)
    {
      var auths = await _context.ProfileAccesses.Where(p => p.ProfileId == profile.Id && p.UserId == user.Id && p.AccessType == accessType).ToListAsync();
      if (auths == null)
      {
        return false;
      }
      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveAuthorization(Guid profileAccessId)
    {
      var profileAccess = await _context.ProfileAccesses.FirstOrDefaultAsync(p => p.Id == profileAccessId);
      if (profileAccess == null)
      {
        return false;
      }
      _context.ProfileAccesses.Remove(profileAccess);
      int result = await _context.SaveChangesAsync();
      return result == 1;
    }

    public async Task<bool> ClearAuthorizations(Profile profile)
    {
      var auths = await _context.ProfileAccesses.Where(p => p.ProfileId == profile.Id).ToListAsync();
      if (auths == null)
      {
        return false;
      }
      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ClearAuthorizations(ApplicationUser user)
    {
      var auths = await _context.ProfileAccesses.Where(p => p.UserId == user.Id).ToListAsync();
      if (auths == null)
      {
        return false;
      }
      _context.ProfileAccesses.RemoveRange(auths);
      return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SetProfileOwner(Profile profile, ApplicationUser user)
    {
      ApplicationUser? _user = await _context.Users.Include(p => p.OwnedProfiles).FirstOrDefaultAsync(p => p.Id == user.Id);
      Profile? _profile = await _context.Profiles.FirstOrDefaultAsync(p => p.Id == profile.Id);
      if (_profile == null || _user == null)
      {
        return false;
      }
      ApplicationUser? currentUser = _profile.Owner;
      if (currentUser != null && currentUser.OwnedProfiles != null)
      {
        currentUser.OwnedProfiles.Remove(_profile);
        _context.Users.Update(currentUser);
      }
      _profile.Owner = user;
      _context.Profiles.Update(_profile);
      if (user.OwnedProfiles != null)
      {
        user.OwnedProfiles.Add(_profile);
        _context.Users.Update(_user);
      }
      await _context.SaveChangesAsync();
      return true;
    }

  }
}
