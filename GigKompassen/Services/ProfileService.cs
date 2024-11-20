using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class ProfileService
  {
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<BaseProfile>> GetAllAsync(int? skip = null, int? take = null, ProfileQueryOptions? queryOptions = null)
    {
      var query = _context.Profiles.AsQueryable();

      if (skip.HasValue)
        query = query.Skip(skip.Value);

      if (take.HasValue)
        query = query.Take(take.Value);

      if(queryOptions != null)
        query = queryOptions.Apply(query);
      
      return await query.ToListAsync();
    }

    public async Task<BaseProfile?> GetByNameAsync(string name)
    {
      return await _context.Profiles.FirstOrDefaultAsync(p => p.Name == name);
    }

    public async Task<BaseProfile?> GetByIdAsync(Guid id)
    {
      return await _context.Profiles.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<BaseProfile>> GetProfilesOwnedByUserAsync(Guid userId)
    {
      return await _context.Profiles.Where(p => p.OwnerId == userId).ToListAsync();
    }

  }

  public class ProfileQueryOptions
  {
    public bool IncludeOwner { get; } = false;
    public bool IncludeMediaGallery { get; } = false;

    public ProfileTypes? ProfileType { get; init; } = null;

    public ProfileQueryOptions(bool includeOwner = false, bool includeMediaGallery = false, ProfileTypes? profileType = null)
    {
      IncludeOwner = includeOwner;
      IncludeMediaGallery = includeMediaGallery;
      ProfileType = profileType;
    }

    public IQueryable<BaseProfile> Apply(IQueryable<BaseProfile> query)
    {
      if (IncludeOwner)
        query = query.Include(sp => sp.Owner);

      if (IncludeMediaGallery)
        query = query.Include(sp => sp.MediaGalleryOwner);

      if (ProfileType != null)
        query = query.Where(p => p.ProfileType == ProfileType);

      return query;
    }
  }
}
