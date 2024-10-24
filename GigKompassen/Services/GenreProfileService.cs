using GigKompassen.Data;

using Microsoft.EntityFrameworkCore;
namespace GigKompassen.Services
{
  public class GenreProfileService
  {
    private readonly ApplicationDbContext _context;
    private readonly GenreService _genreService;

    public GenreProfileService(ApplicationDbContext context, GenreService genreService)
    {
      _context = context;
      _genreService = genreService;
    }

    public async Task<bool> ArtistHasGenreAsync(Guid artistId, string genreName)
    {
      return await _context.ArtistProfiles.Include(a => a.Genres).AnyAsync(a => a.Genres.Any(g => g.Name.Equals(genreName)));
    }

    public async Task<bool> ArtistHasGenreAsync(Guid artistId, Guid genreId)
    {
      return await _context.ArtistProfiles.Include(a => a.Genres).AnyAsync(a => a.Genres.Any(g => g.Id == genreId));
    }

    public async Task<bool> SceneHasGenreAsync(Guid sceneId, string genreName)
    {
      return await _context.SceneProfiles.Include(a => a.Genres).AnyAsync(a => a.Genres.Any(g => g.Name.Equals(genreName)));
    }

    public async Task<bool> SceneHasGenreAsync(Guid sceneId, Guid genreId)
    {
      return await _context.SceneProfiles.Include(a => a.Genres).AnyAsync(a => a.Genres.Any(g => g.Id == genreId));
    }

    public async Task<bool> GenreHasArtistAsync(Guid genreId, Guid artistId)
    {
      return await _context.Genres.Include(g => g.ArtistProfiles).AnyAsync(g => g.ArtistProfiles.Any(a => a.Id == artistId));
    }

    public async Task<bool> GenreHasSceneAsync(Guid genreId, Guid sceneId)
    {
      return await _context.Genres.Include(g => g.SceneProfiles).AnyAsync(g => g.SceneProfiles.Any(a => a.Id == sceneId));
    }

    public async Task<bool> AddGenreToArtist(Guid artistId, string genreName)
    {
      var artist = await _context.ArtistProfiles.Include(a => a.Genres).AsTracking().FirstOrDefaultAsync(a => a.Id == artistId);
      if (artist == null)
        return false;

      var genre = await _genreService.AddOrGetGenreAsync(genreName);
      artist.Genres.Add(genre);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> AddGenreToScene(Guid sceneId, string genreName)
    {
      var scene = await _context.SceneProfiles.Include(s => s.Genres).AsTracking().FirstOrDefaultAsync(s => s.Id == sceneId);
      if (scene == null)
        return false;

      var genre = await _genreService.AddOrGetGenreAsync(genreName);
      scene.Genres.Add(genre);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> RemoveGenreFromArtist(Guid artistId, string genreName)
    {
      var artist = await _context.ArtistProfiles.Include(a => a.Genres).AsTracking().FirstOrDefaultAsync(a => a.Id == artistId);
      if (artist == null)
        return false;

      var genreToRemove = artist.Genres.FirstOrDefault(g => genreName.Equals(genreName, StringComparison.OrdinalIgnoreCase));
      if(genreToRemove == null)
        return false;

      artist.Genres.Remove(genreToRemove);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> RemoveGenreFromScene(Guid sceneId, string genreName)
    {
      var scene = await _context.SceneProfiles.Include(s => s.Genres).AsTracking().FirstOrDefaultAsync(s => s.Id == sceneId);
      if (scene == null)
        return false;

      var genreToRemove = scene.Genres.FirstOrDefault(g => g.Name.Equals(genreName, StringComparison.OrdinalIgnoreCase));
      if (genreToRemove == null)
        return false;

      scene.Genres.Remove(genreToRemove);
      await _context.SaveChangesAsync();
      return true;
    }
  }
}
