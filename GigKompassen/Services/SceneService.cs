using GigKompassen.Data;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;


namespace GigKompassen.Services
{
  public class SceneService
  {
    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;

    public delegate void GenreEventHandler(object? sender, SceneProfile sceneProfile, Genre genre);
    public delegate void GenresEventHandler(object? sender, SceneProfile SceneProfile, List<Genre> genres);

    public event AsyncEventHandler<SceneProfile> OnCreateSceneProfile;
    public event AsyncEventHandler<SceneProfile> OnUpdateSceneProfile;
    public event AsyncEventHandler<SceneProfile> OnDeleteSceneProfile;
    public event AsyncEventHandler<GenreEventArgs> OnGenreAdded;
    public event AsyncEventHandler<GenresEventArgs> OnGenresAdded;
    public event AsyncEventHandler<GenreEventArgs> OnGenreRemoved;
    public event AsyncEventHandler<GenresEventArgs> OnGenresRemoved;

    public SceneService(ApplicationDbContext context, UserService userService, GenreService genreService, MediaService mediaService)
    {
      _context = context;
      _userService = userService;
      _genreService = genreService;
      _mediaService = mediaService;

      _userService.OnDeleteUser += async (sender, user) =>
      {
        var profiles = await GetSceneProfilesOwnerByUserAsync(user.Id);
        await Task.WhenAll(profiles.Select(p => DeleteAsync(p.Id)));
      };
    }
    #region Getters
    public async Task<ICollection<SceneProfile>> GetAllAsync()
    {
      var sceneProfiles = await _context.SceneProfiles.ToListAsync();
      return sceneProfiles;
    }

    public async Task<SceneProfile?> GetAsync(Guid id)
    {
      var sceneProfiles = await _context.SceneProfiles.FirstOrDefaultAsync(p => p.Id == id);
      return sceneProfiles;
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(string genreName)
    {
      var genre = await _genreService.GetGenreAsync(genreName);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetSceneProfilesUsingGenre(genre);
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(Guid genreId)
    {
      var genre = await _genreService.GetGenreAsync(genreId);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetSceneProfilesUsingGenre(genre);
    }

    public async Task<List<SceneProfile>> GetSceneProfilesUsingGenre(Genre genre)
    {
      return await _context.SceneProfiles
                .Include(ap => ap.Genres)
                .Where(ap => ap.Genres!.Contains(genre))
                .ToListAsync();
    }

    public async Task<List<SceneProfile>> GetSceneProfilesOwnerByUserAsync(Guid userId)
    {
      if (!await _context.Users.AnyAsync(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      return await _context.SceneProfiles.Where(p => p.OwnerId == userId).ToListAsync();
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

      if(OnCreateSceneProfile != null)
        await OnCreateSceneProfile.InvokeAsync(this, profile);

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

      if (OnUpdateSceneProfile != null)
        await OnUpdateSceneProfile.InvokeAsync(this, profile);

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

      if(OnDeleteSceneProfile != null)
        await OnDeleteSceneProfile.InvokeAsync(this, sceneProfile);
      
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

      if(OnGenreAdded != null)
        await OnGenreAdded.InvokeAsync(this, new(sceneProfile, genre));

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

      if(OnGenresAdded != null)
        await OnGenresAdded.InvokeAsync(this, new (sceneProfile, genres));

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
        if(OnGenreRemoved != null)
          await OnGenreRemoved.InvokeAsync(this, new(sceneProfile, genre));

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

      if(OnGenresRemoved != null)
        await OnGenresRemoved.InvokeAsync(this, new(sceneProfile, toRemove));

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
    public record class GenreEventArgs(SceneProfile SceneProfile, Genre Genre);
    public record class GenresEventArgs(SceneProfile SceneProfile, List<Genre> Genres);
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
}
