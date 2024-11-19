using GigKompassen.Data;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Services
{
  public class ProfileService
  {
    private readonly ApplicationDbContext _context;

    public ProfileService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<List<BaseProfile>> GetAllAsync(int? skip = null, int? take = null)
    {
      var query = _context.Profiles.AsQueryable();

      if (skip.HasValue)
      {
        query = query.Skip(skip.Value);
      }

      if (take.HasValue)
      {
        query = query.Take(take.Value);
      }

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
}
