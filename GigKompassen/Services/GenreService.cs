using GigKompassen.Data;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;

namespace GigKompassen.Services
{
  public class GenreService
  {
    private readonly ApplicationDbContext _context;

    public event AsyncEventHandler<Genre> OnGenreCreated;
    public event AsyncEventHandler<List<Genre>> OnGenresCreated;
    public event AsyncEventHandler<Genre> OnGenreRemoved;
    public event AsyncEventHandler<List<Genre>> OnGenresRemoved;

    public GenreService(ApplicationDbContext context)
    {
      _context = context;
    }

    #region HasGenre
    public async Task<bool> HasGenreAsync(string genre)
    {
      return await _context.Genres.AnyAsync(g => g.Name.Equals(genre));
    }

    public async Task<bool> HasGenreAsync(Guid id)
    {
      return await _context.Genres.AnyAsync(g => g.Id == id);
    }
    #endregion

    #region Getters
    public async Task<Genre?> GetGenreAsync(Guid id)
    {
      return await _context.Genres.FirstOrDefaultAsync(g => g.Id == id);
    }

    public async Task<Genre?> GetGenreAsync(string genre)
    {
      return await _context.Genres.FirstOrDefaultAsync(g => g.Name.Equals(genre));
    }

    public async Task<List<Genre>> GetGenresAsync(IEnumerable<Guid> genres)
    {
      return await _context.Genres.Where(g => genres.Contains(g.Id)).ToListAsync();
    }

    public async Task<List<Genre>> GetGenresAsync(IEnumerable<string> genres)
    {
      return await _context.Genres.Where(g => genres.Contains(g.Name)).ToListAsync();
    }

    #endregion

    #region Creators
    public async Task<Genre> CreateGenreAsync(string genreName)
    {
      Genre genre = new Genre() { Id = Guid.NewGuid(), Name = genreName};

      if(await _context.Genres.AnyAsync(g => g.Name.Equals(genreName)))
        throw new ArgumentException("Genre already exists", nameof(genreName));

      await _context.Genres.AddAsync(genre);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create genre");

      if(OnGenreCreated != null)
        await OnGenreCreated.InvokeAsync(this, genre);

      return genre;
    }

    public async Task<List<Genre>> CreateGenresAsync(IEnumerable<string> genreNames)
    {
      List<Genre> genres = new List<Genre>();
      foreach(string genreName in genreNames)
      {
        if(_context.Genres.Any(g => g.Name.Equals(genreName)))
          continue;
        Genre genre = new Genre() { Id = Guid.NewGuid(), Name = genreName };
        await _context.Genres.AddAsync(genre);
        genres.Add(genre);
      }
      await _context.SaveChangesAsync();
      
      if (OnGenresCreated != null)
        await OnGenresCreated.InvokeAsync(this, genres);

      return genres;
    }

    public async Task<Genre> GetOrCreateGenreAsync(string genreName)
    {
      Genre? genre = await GetGenreAsync(genreName);
      if (genre != null)
        return genre;

      return await CreateGenreAsync(genreName);
    }

    public async Task<List<Genre>> GetOrCreateGenresAsync(IEnumerable<string> genres)
    {
      var existingGenres = await GetGenresAsync(genres);

      var newGenres = await CreateGenresAsync(genres);

      return existingGenres.Concat(newGenres).ToList();
    }
    #endregion

    #region Deleters
    public async Task<bool> RemoveGenreAsync(Guid id)
    {
      Genre? toRemove = await GetGenreAsync(id);
      if (toRemove == null)
        return false;

      _context.Genres.Remove(toRemove!);
      var result = await _context.SaveChangesAsync();

      if (result == 1 && OnGenreRemoved != null)
        await OnGenreRemoved.InvokeAsync(this, toRemove!);

      return result == 1;
    }

    public async Task<int> RemoveGenresAsync(IEnumerable<Guid> ids)
    {
      var existingGenres = await GetGenresAsync(ids);

      _context.Genres.RemoveRange(existingGenres);
      var result = await _context.SaveChangesAsync();

      if (result > 0 && OnGenresRemoved != null)
        await OnGenresRemoved.InvokeAsync(this, existingGenres);

      return result;
    }

    public async Task<bool> RemoveGenreAsync(string name)
    {
      Genre? toRemove = await GetGenreAsync(name);
      if (toRemove == null)
        return false;

      _context.Genres.Remove(toRemove!);
      var result = await _context.SaveChangesAsync();

      if (result == 1 && OnGenreRemoved != null)
        await OnGenreRemoved.InvokeAsync(this, toRemove!);

      return result == 1;
    }

    public async Task<int> RemoveGenresAsync(IEnumerable<string> genres)
    {
      var existingGenres = await GetGenresAsync(genres);
      _context.Genres.RemoveRange(existingGenres);

      var result = await _context.SaveChangesAsync();
      if (result > 0 && OnGenresRemoved != null)
        await OnGenresRemoved.InvokeAsync(this, existingGenres);

      return result;
    }

    public async Task<bool> RemoveGenreAsync(Genre genre)
    {
      _context.Genres.Remove(genre);
      var result = await _context.SaveChangesAsync();

      if (result == 1 && OnGenreRemoved != null)
        await OnGenreRemoved.InvokeAsync(this, genre);

      return result == 1;
    }

    public async Task<int> RemoveGenresAsync(IEnumerable<Genre> genres)
    {
      _context.Genres.RemoveRange(genres);

      var result = await _context.SaveChangesAsync();

      if (result > 0 && OnGenresRemoved != null)
        await OnGenresRemoved.InvokeAsync(this, genres.ToList());

      return result;
    }
    #endregion
  }
}
