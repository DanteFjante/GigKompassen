using GigKompassen.Data;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;

namespace GigKompassen.Services
{
  public class ManagerService
  {
    private readonly ApplicationDbContext _context;
    private readonly MediaService _mediaService;

    public event AsyncEventHandler<ManagerProfile> OnCreateManagerProfile;
    public event AsyncEventHandler<ManagerProfile> OnUpdateManagerProfile;
    public event AsyncEventHandler<ManagerProfile> OnDeleteManagerProfile;

    public ManagerService(ApplicationDbContext context, MediaService mediaService) 
    {
      _context = context;
      _mediaService = mediaService;

    }

    #region Getters
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

    public async Task<List<ManagerProfile>> GetManagerProfilesOwnerByUserAsync(Guid userId)
    {
      if (!await _context.Users.AnyAsync(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      return await _context.ManagerProfiles.Where(p => p.OwnerId == userId).ToListAsync();
    }
    #endregion

    #region Creators
    public async Task<ManagerProfile?> CreateAsync(Guid userId, CreateManagerDto dto)
    {

      if (dto == null)
        throw new ArgumentNullException(nameof(dto));


      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      ManagerProfile profile = dto.ToManagerProfile();

      profile.Owner = user;
      profile.OwnerId = userId;

      MediaGalleryOwner mediaGalleryOwner = new MediaGalleryOwner()
      {
        Id = Guid.NewGuid(),
        Galleries = new List<MediaGallery>()
      };

      profile.MediaGalleryOwner = mediaGalleryOwner;
      profile.MediaGalleryOwnerId = mediaGalleryOwner.Id;

      if(OnCreateManagerProfile != null)
        await OnCreateManagerProfile.Invoke(this, profile);

      await _context.ManagerProfiles.AddAsync(profile);
      
      if(await _context.SaveChangesAsync() == 0)
        throw new Exception("Failed to create ManagerProfile");

      return profile;
    }
    #endregion

    #region Updaters
    public async Task<ManagerProfile> UpdateAsync(Guid id, UpdateManagerDto dto)
    {
      ManagerProfile? profile = await _context.ManagerProfiles.FirstOrDefaultAsync(mp => mp.Id == id);

      if(profile == null)
        throw new KeyNotFoundException("Profile not found");

      dto.UpdateManager(profile);

      if(OnUpdateManagerProfile != null)
        await OnUpdateManagerProfile.Invoke(this, profile);

      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to update ManagerProfile");

      return profile;
    }
    #endregion

    #region Deleters
    public async Task<bool> DeleteAsync(Guid id)
    {
      var profile = await _context.ManagerProfiles.FindAsync(id);
      if (profile == null)
        return false;

      if(OnDeleteManagerProfile != null)
        await OnDeleteManagerProfile.Invoke(this, profile);

      _context.ManagerProfiles.Remove(profile);

      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete SceneProfile");

      await _mediaService.DeleteMediaGalleryOwnerAsync(profile.MediaGalleryOwnerId);

      return true;
    }
    #endregion
  }

  public record class CreateManagerDto(string Name, string? Description, string? Location, bool PublicProfile = true)
  {
    public ManagerProfile ToManagerProfile()
    {
      if(string.IsNullOrWhiteSpace(Name))
        throw new ArgumentException("Name is required");

      return new ManagerProfile()
      {
        Name = Name,
        Description = Description ?? string.Empty,
        Location = Location ?? string.Empty,
        Public = PublicProfile
      };
    }

    public static CreateManagerDto FromManagerProfile(ManagerProfile manager)
    {
      return new CreateManagerDto(manager.Name, manager.Description, manager.Location, manager.Public);
    }
  }

  public record class UpdateManagerDto(string? Name = null, string? Description = null, string? Location = null, bool? publicProfile = null)
  {
    public void UpdateManager(ManagerProfile profile)
    {
      if (!string.IsNullOrWhiteSpace(Name))
        profile.Name = Name;

      if (!string.IsNullOrWhiteSpace(Description))
        profile.Description = Description;

      if (!string.IsNullOrWhiteSpace(Location))
        profile.Location = Location;

      if (publicProfile.HasValue)
        profile.Public = publicProfile.Value;
    }

    public static UpdateManagerDto FromManagerProfile(ManagerProfile manager)
    {
      return new UpdateManagerDto(manager.Name, manager.Description, manager.Location, manager.Public);
    }
  }
}
