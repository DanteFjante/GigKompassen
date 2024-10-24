using GigKompassen.Data;
using GigKompassen.Dto.Profiles;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class ManagerProfileService
  {
    private readonly ApplicationDbContext _context;

    public ManagerProfileService(ApplicationDbContext context) 
    {
      _context = context;
    }

    public async Task<ICollection<ManagerProfile>> GetAllAsync()
    {
      var profiles = await _context.ManagerProfiles.ToListAsync();
      return profiles;
    }

    public async Task<ManagerProfile?> GetAsync(Guid id)
    {
      var profile = await _context.ManagerProfiles.FirstOrDefaultAsync(p => p.Id == id);
      return profile;
    }

    public async Task<ManagerProfile?> CreateAsync(ManagerProfileDto managerProfile)
    {

      if (managerProfile == null)
        throw new ArgumentNullException(nameof(managerProfile));

      if (string.IsNullOrEmpty(managerProfile.Name))
        throw new ArgumentException("Name is required");

      ManagerProfile profile = new ManagerProfile()
      {
        Id = managerProfile.Id ?? Guid.NewGuid(),
        Name = managerProfile.Name,
        Description = managerProfile.Description,
        Location = managerProfile.Location
      };
      var ret = (await _context.ManagerProfiles.AddAsync(profile)).Entity;
      await _context.SaveChangesAsync();
      return ret;
    }

    public async Task<ManagerProfile> UpdateAsync(Guid id, ManagerProfileDto dto)
    {
      ManagerProfile? profile = await _context.ManagerProfiles.FirstOrDefaultAsync(mp => mp.Id == id);

      if(profile == null)
        throw new KeyNotFoundException("Profile not found");

      profile.Name = dto.Name ?? profile.Name;
      profile.Description = dto.Description ?? profile.Description;
      profile.Location = dto.Location ?? profile.Location;

      _context.ManagerProfiles.Update(profile);
      await _context.SaveChangesAsync();
      return profile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      var profile = await _context.ManagerProfiles.FindAsync(id);
      if (profile == null)
        return false;
      _context.ManagerProfiles.Remove(profile);
      var result = await _context.SaveChangesAsync();
      return result == 1;
    }

  }


}
