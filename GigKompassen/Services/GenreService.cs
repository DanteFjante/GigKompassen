using GigKompassen.Data;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class GenreService
  {
    private readonly ApplicationDbContext _context;

    public GenreService(ApplicationDbContext context)
    {
      _context = context;
    }

    public async Task<bool> HasGenreAsync(string genre)
    {
      return await _context.Genres.AnyAsync(g => g.Name.Equals(genre));
    }

    public async Task<bool> HasGenreAsync(Guid id)
    {
      return await _context.Genres.AnyAsync(g => g.Id == id);
    }

    public async Task<Genre?> GetGenreAsync(Guid id, GenreQueryOptions? options = null)
    {
      var query = _context.Genres.AsQueryable();

      if (options != null)
      {
        query = options.Apply(query);
      }

      return await query.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Genre?> GetGenreAsync(string genre, GenreQueryOptions? options = null)
    {
      var query = _context.Genres.AsQueryable();

      if (options != null)
      {
        query = options.Apply(query);
      }
      return await query.FirstOrDefaultAsync(g => g.Name.Equals(genre));
    }

    public async Task<List<Genre>> GetGenresAsync(IEnumerable<string> genres, GenreQueryOptions? options = null)
    {
      var query = _context.Genres.AsQueryable();
      if (options != null)
      {
        query = options.Apply(query);
      }
      return await query.Where(g => genres.Any(s => s.Equals(g.Name))).ToListAsync();
    }

    public async Task<Genre?> AddOrGetGenreAsync(string genre, GenreQueryOptions? options = null)
    {

      if (!await HasGenreAsync(genre))
      {
        await _context.AddAsync(new Genre { Name = genre });
        await _context.SaveChangesAsync();
      }

      return await GetGenreAsync(genre, options);
    }

    public async Task<List<Genre>> AddOrGetGenresAsync(IEnumerable<string> genres, GenreQueryOptions? options = null)
    {
      var query = _context.Genres.AsQueryable();

      if(options != null)
      {
        query = options.Apply(query);
      }

      var existingGenres = await query.Where(g => genres.Any(s => s.Equals(g.Name))).ToListAsync();

      var newGenres = genres.Except(existingGenres
        .Select(p => p.Name))
        .Select(p => 
          new Genre() 
          {
            Id = Guid.NewGuid(),
            Name = p
          })
      .ToList();

      await Task.WhenAll(newGenres.Select(async g => await _context.Genres.AddAsync(g)));
      await _context.SaveChangesAsync();

      return await GetGenresAsync(genres, options);
    }

    public async Task<bool> RemoveGenreAsync(Guid id)
    {
      if(!await HasGenreAsync(id))
        return false;
      Genre? toRemove = await GetGenreAsync(id);
      _context.Genres.Remove(toRemove!);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> RemoveGenreAsync(string name)
    {
      if (!await HasGenreAsync(name))
        return false;
      Genre? toRemove = await GetGenreAsync(name);
      _context.Genres.Remove(toRemove!);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task RemoveGenresAsync(IEnumerable<string> genres)
    {
      var existingGenres = await _context.Genres.Where(g => genres.Any(s => s.Equals(g.Name))).ToListAsync();
      _context.Genres.RemoveRange(existingGenres);
      await _context.SaveChangesAsync();
    }

    public async Task RemoveGenresAsync(IEnumerable<Guid> ids)
    {
      var existingGenres = await _context.Genres.Where(g => ids.Contains(g.Id)).ToListAsync();
      _context.Genres.RemoveRange(existingGenres);
      await _context.SaveChangesAsync();
    }

  }

  public class GenreQueryOptions
  {
    public bool IncludeArtistProfiles { get; set; }
    public bool IncludeSceneProfiles { get; set; }

    public GenreQueryOptions(bool includeArtistProfiles = false, bool includeSceneProfiles = false)
    {
      IncludeArtistProfiles = includeArtistProfiles;
      IncludeSceneProfiles = includeSceneProfiles;
    }

    public IQueryable<Genre> Apply(IQueryable<Genre> query)
    {
      if (IncludeArtistProfiles)
        query = query.Include(g => g.ArtistProfiles);

      if (IncludeSceneProfiles)
        query = query.Include(g => g.SceneProfiles);

      return query;
    }
  }
}
