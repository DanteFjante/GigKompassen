using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class SceneService
  {
    private readonly ApplicationDbContext _context;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;

    public SceneService(ApplicationDbContext context, GenreService genreService, MediaService mediaService)
    {
      _context = context;
      _genreService = genreService;
      _mediaService = mediaService;

    }
    #region Getters
    public async Task<List<SceneProfile>> GetAllAsync(int? skip = null, int? take = null, SceneProfileQueryOptions? options = null)
    {
      var query = _context.SceneProfiles.AsQueryable();

      if (skip.HasValue)
        query = query.Skip(skip.Value);

      if (take.HasValue)
        query = query.Take(take.Value);

      if (options != null)
        query = options.Apply(query);

      return await query.ToListAsync();
    }

    public async Task<SceneProfile?> GetAsync(Guid id, SceneProfileQueryOptions? options = null)
    {
      var query = _context.SceneProfiles.AsQueryable();

      if (options != null)
        query = options.Apply(query);

      var sceneProfiles = await query.FirstOrDefaultAsync(p => p.Id == id);

      return sceneProfiles;
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(string genreName, SceneProfileQueryOptions? queryOptions = null)
    {
      var genre = await _genreService.GetGenreAsync(genreName);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetSceneProfilesUsingGenre(genre, queryOptions);
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(Guid genreId, SceneProfileQueryOptions? queryOptions = null)
    {
      var genre = await _genreService.GetGenreAsync(genreId);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetSceneProfilesUsingGenre(genre);
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(Genre genre, SceneProfileQueryOptions? queryOptions = null)
    {
      var query = _context.SceneProfiles
          .Include(ap => ap.Genres)
          .Where(ap => ap.Genres!.Contains(genre));

      if (queryOptions != null)
        query = queryOptions.Apply(query);

      return await query.ToListAsync();
    }

    public async Task<List<SceneProfile>> GetSceneProfilesOwnerByUserAsync(Guid userId, SceneProfileQueryOptions? queryOptions = null)
    {
      if (!await _context.Users.AnyAsync(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      var query = _context.SceneProfiles
          .Where(p => p.OwnerId == userId);

      if (queryOptions != null)
        query = queryOptions.Apply(query);

      return await query.ToListAsync();
    }
    #endregion

    #region Creators
    public async Task<SceneProfile> CreateAsync(Guid userId, CreateSceneDto dto, List<string>? genreNames)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      SceneProfile profile = dto.ToSceneProfile();

      profile.Owner = user;
      profile.OwnerId = user.Id;

      MediaGalleryOwner mediaGalleryOwner = new MediaGalleryOwner()
      {
        Id = Guid.NewGuid(),
        Galleries = new List<MediaGallery>()
      };

      profile.MediaGalleryOwner = mediaGalleryOwner;
      profile.MediaGalleryOwnerId = mediaGalleryOwner.Id;

      if (genreNames != null && genreNames.Any())
      {
        var genres = await _genreService.GetOrCreateGenresAsync(genreNames);
        profile.Genres!.AddRange(genres);
      }

      await _context.SceneProfiles.AddAsync(profile);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create profile");

      return profile;
    }
    #endregion

    #region Updaters
    public async Task<SceneProfile> UpdateAsync(Guid id, UpdateSceneDto dto, List<string>? genreNames)
    {
      SceneProfile? profile = await _context.SceneProfiles.FindAsync(id);
      if (profile == null)
        throw new KeyNotFoundException("Profile not found");

      dto.UpdateScene(profile);

      if (genreNames != null && genreNames.Any())
      {
        List<string> GenresToRemove = new();
        List<string> GenresToKeep = new();
        List<string> NewGenres;
        if (profile.Genres != null)
        {
          GenresToRemove = profile.Genres.Select(g => g.Name).SkipWhile(g => genreNames.Contains(g)).ToList();
          GenresToKeep = profile.Genres.Select(g => g.Name).TakeWhile(g => genreNames.Contains(g)).ToList();
        }

        NewGenres = genreNames.Except(GenresToKeep).ToList();

        foreach (var genreName in GenresToRemove)
        {
          await RemoveGenreAsync(profile.Id, genreName);
        }

        foreach (var genreName in NewGenres)
        {
          await AddGenreAsync(profile.Id, genreName);
        }
      }

      await _context.SaveChangesAsync();
      return profile;
    }
    #endregion

    #region Deleters
    public async Task<bool> DeleteAsync(Guid id)
    {
      var sceneProfile = await _context.SceneProfiles.Include(sp => sp.MediaGalleryOwner).FirstOrDefaultAsync(sp => sp.Id == id);
      if (sceneProfile == null)
        return false;

      if (sceneProfile.Genres != null && sceneProfile.Genres.Any())
        await RemoveGenresAsync(sceneProfile.Id, sceneProfile.Genres.Select(g => g.Name).ToList());
      
      _context.SceneProfiles.Remove(sceneProfile);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete SceneProfile");

      if (sceneProfile.MediaGalleryOwner != null)
      {
        await _mediaService.DeleteMediaGalleryOwnerAsync(sceneProfile.MediaGalleryOwner.Id);
      }

      return true;
    }
    #endregion

    #region Genres
    public async Task<Genre> AddGenreAsync(Guid sceneProfileId, string genreName)
    {
      var sceneProfile = await _context.SceneProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == sceneProfileId);

      if (sceneProfile == null)
        throw new KeyNotFoundException("SceneProfile not found");

      var genre = await _genreService.GetOrCreateGenreAsync(genreName);

      sceneProfile.Genres!.Add(genre);
      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add Genre");

      return genre;
    }

    public async Task<List<Genre>> AddGenresAsync(Guid sceneProfileId, List<string> genreNames)
    {
      var sceneProfile = await _context.SceneProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == sceneProfileId);

      if (sceneProfile == null)
        throw new KeyNotFoundException("SceneProfile not found");

      var genres = await _genreService.GetOrCreateGenresAsync(genreNames);

      sceneProfile.Genres!.AddRange(genres);
      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add Genres");

      return genres;
    }

    public async Task<bool> RemoveGenreAsync(Guid sceneProfileId, string genreName)
    {
      var sceneProfile = await _context.SceneProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == sceneProfileId);

      if (sceneProfile == null)
        throw new KeyNotFoundException("SceneProfile not found");

      var genre = await _genreService.GetGenreAsync(genreName);

      if (genre == null || sceneProfile.Genres == null || !sceneProfile.Genres.Any())
        return false;

      if (sceneProfile.Genres.Contains(genre))
      {

        sceneProfile.Genres.Remove(genre);
        if (await _context.SaveChangesAsync() == 0)
          throw new DbUpdateException("Failed to remove Genre");
      }
      else
        return false;

      var sceneProfilesUsingGenre = await GetSceneProfilesUsingGenre(genre);

      if (!sceneProfilesUsingGenre.Any())
      {
        await _genreService.RemoveGenreAsync(genre);
      }

      return true;
    }

    public async Task<int> RemoveGenresAsync(Guid sceneProfileId, List<string> genreNames)
    {
      var sceneProfile = await _context.SceneProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == sceneProfileId);
      if (sceneProfile == null)
        throw new KeyNotFoundException("SceneProfile not found");

      if (sceneProfile.Genres == null || !sceneProfile.Genres.Any())
        return 0;

      var toRemove = sceneProfile.Genres.Where(g => genreNames.Contains(g.Name)).ToList();

      foreach (var genre in toRemove)
      {
        sceneProfile.Genres.Remove(genre);
      }
      int result = await _context.SaveChangesAsync();

      foreach (var genre in toRemove)
      {
        var sceneProfilesUsingGenre = await GetSceneProfilesUsingGenre(genre);

        if (!sceneProfilesUsingGenre.Any())
        {
          await _genreService.RemoveGenreAsync(genre);
        }
      }

      return result;
    }
    #endregion
  }

  public record class CreateSceneDto(string Name, string? Address, string? VenueType, string? ContactInfo, int Capacity, string? Bio, string? Description, string? Amenities, string? OpeningHours, bool PublicProfile = true)
  {
    public SceneProfile ToSceneProfile()
    {
      if(string.IsNullOrEmpty(Name))
        throw new ArgumentException("Name is required");

      return new SceneProfile()
      {
        Id = Guid.NewGuid(),
        Name = Name,
        Address = Address ?? string.Empty,
        VenueType = VenueType ?? string.Empty,
        ContactInfo = ContactInfo ?? string.Empty,
        Capacity = Capacity,
        Bio = Bio ?? string.Empty,
        Description = Description ?? string.Empty,
        Amenities = Amenities ?? string.Empty,
        OpeningHours = OpeningHours ?? string.Empty,
        Public = PublicProfile,
      };
    }

    public static CreateSceneDto FromSceneProfile(SceneProfile scene)
    {
      return new CreateSceneDto(scene.Name, scene.Address, scene.VenueType, scene.ContactInfo, scene.Capacity, scene.Bio, scene.Description, scene.Amenities, scene.OpeningHours, scene.Public);
    }
  }

  public record class UpdateSceneDto(string? Name = null, string? Address = null, string? VenueType = null, string? ContactInfo = null, int? Capacity = null, string? Bio = null, string? Description = null, string? Amenities = null, string? OpeningHours = null, bool? publicProfile = null)
  {
    public void UpdateScene(SceneProfile profile)
    {
      if(!string.IsNullOrWhiteSpace(Name))
        profile.Name = Name;

      if (!string.IsNullOrWhiteSpace(Address))
        profile.Address = Address;

      if (!string.IsNullOrWhiteSpace(VenueType))
        profile.VenueType = VenueType;

      if (!string.IsNullOrWhiteSpace(ContactInfo))
        profile.ContactInfo = ContactInfo;

      if (Capacity != null)
        profile.Capacity = Capacity.Value;

      if (!string.IsNullOrWhiteSpace(Bio))
        profile.Bio = Bio;

      if (!string.IsNullOrWhiteSpace(Description))
        profile.Description = Description;

      if (!string.IsNullOrWhiteSpace(Amenities))
        profile.Amenities = Amenities;

      if (!string.IsNullOrWhiteSpace(OpeningHours))
        profile.OpeningHours = OpeningHours;

      if (publicProfile.HasValue)
        profile.Public = publicProfile.Value;
    }

    public static UpdateSceneDto FromSceneProfile(SceneProfile scene)
    {
      return new UpdateSceneDto(scene.Name, scene.Address, scene.VenueType, scene.ContactInfo, scene.Capacity, scene.Bio, scene.Description, scene.Amenities, scene.OpeningHours, scene.Public);
    }
  }

  public class SceneProfileQueryOptions : ProfileQueryOptions
  {
    public bool IncludeGenres { get; set; } = false;

    public SceneProfileQueryOptions(bool includeOwner = false, bool includeMediaGallery = false, bool includeGenres = false) : base(includeOwner, includeMediaGallery)
    {
      IncludeGenres = includeGenres;
      ProfileType = ProfileTypes.Scene;
    }

    public IQueryable<SceneProfile> Apply(IQueryable<SceneProfile> query)
    {
      if (IncludeOwner)
        query = query.Include(sp => sp.Owner);

      if (IncludeMediaGallery)
        query = query.Include(sp => sp.MediaGalleryOwner);

      if(IncludeGenres)
        query = query.Include(sp => sp.Genres);

      return query;
    }
  }
}
