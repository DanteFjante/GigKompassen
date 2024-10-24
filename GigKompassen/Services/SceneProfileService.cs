using GigKompassen.Data;
using GigKompassen.Dto.Profiles;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;


namespace GigKompassen.Services
{
  public class SceneProfileService
  {
    private readonly ApplicationDbContext _context;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;

    public SceneProfileService(ApplicationDbContext context, GenreService genreService, MediaService mediaService)
    {
      _context = context;
      _genreService = genreService;
      _mediaService = mediaService;
    }

    public async Task<ICollection<SceneProfile>> GetAllAsync(SceneProfileQueryOptions? options = null)
    {
      var query = _context.SceneProfiles.AsQueryable();
      if (options != null)
      {
        query = options.Apply(query);
      }

      var sceneProfiles = await query.ToListAsync();
      return sceneProfiles;
    }

    public async Task<SceneProfile?> GetAsync(Guid id, SceneProfileQueryOptions? options = null)
    {
      var query = _context.SceneProfiles.AsQueryable();
      if (options != null)
      {
        query = options.Apply(query);
      }

      var sceneProfiles = await query.FirstOrDefaultAsync(p => p.Id == id);
      return sceneProfiles;
    }

    public async Task<SceneProfile?> CreateAsync(SceneProfileDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      if (string.IsNullOrEmpty(dto.VenueName))
      {
        throw new ArgumentException("Venue name is required");
      }

      SceneProfile profile = await FromDtoAsync(dto);

      var ret = (await _context.SceneProfiles.AddAsync(profile)).Entity;
      await _context.SaveChangesAsync();
      var galleryOwner = await _mediaService.CreateGalleryAsync(profile);
      ret.GalleryOwner = galleryOwner;
      return ret;
    }

    public async Task<SceneProfile> UpdateAsync(Guid id, SceneProfileDto dto)
    {
      SceneProfile? profile = await _context.SceneProfiles.FindAsync(id);

      if (profile == null)
        throw new KeyNotFoundException("Profile not found");

      profile.Name = dto.VenueName ?? profile.Name;
      profile.Address = dto.Address ?? profile.Address;
      profile.VenueType = dto.VenueType ?? profile.VenueType;
      profile.ContactInfo = dto.ContactInfo ?? profile.ContactInfo;
      profile.Capacity = dto.Capacity ?? profile.Capacity;
      profile.Bio = dto.Bio ?? profile.Bio;
      profile.Description = dto.Description ?? profile.Description;
      profile.Amenities = dto.Amenities ?? profile.Amenities;
      profile.OpeningHours = dto.OpeningHours ?? profile.OpeningHours;

      if (dto.Genres != null)
      {
        var genreNames = dto.Genres.Select(g => g.Name).ToList();

        var genresToRemove = profile.Genres.Where(g => !genreNames.Contains(g.Name)).ToList();
        var genresToAdd = await _genreService.AddOrGetGenresAsync(genreNames.Where(g => !profile.Genres.Any(g => g.Name == g.Name)));

        foreach (var genre in genresToRemove)
        {
          profile.Genres.Remove(genre);
        }
        foreach (var genre in genresToAdd)
        {
          profile.Genres.Add(genre);
        }
      }

      await _context.SaveChangesAsync();
      return profile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      var sceneProfile = await _context.SceneProfiles.Include(sp => sp.GalleryOwner).FirstOrDefaultAsync(sp => sp.Id == id);
      if (sceneProfile == null)
        return false;
      if (sceneProfile.GalleryOwner != null)
      {
        if (sceneProfile.GalleryOwner.Gallery != null)
          _context.MediaGalleries.Remove(sceneProfile.GalleryOwner.Gallery);
        _context.MediaGalleryOwners.Remove(sceneProfile.GalleryOwner);
      }
      _context.SceneProfiles.Remove(sceneProfile);
      var result = await _context.SaveChangesAsync();
      return result == 1;
    }

    private async Task<SceneProfile> FromDtoAsync(SceneProfileDto dto)
    {

      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      if (string.IsNullOrWhiteSpace(dto.VenueName))
        throw new ArgumentException("Name is required.", nameof(dto.VenueName));

      Guid sceneId = dto.Id.HasValue ? dto.Id.Value : Guid.NewGuid();

      List<Genre> genres = new List<Genre>();
      if (dto.Genres != null && dto.Genres.Any())
      {
        var genreNames = dto.Genres.Select(g => g.Name).ToList();
        genres = await _genreService.AddOrGetGenresAsync(genreNames);
      }

      var profile = new SceneProfile
      {
        Id = sceneId,
        Name = dto.VenueName!,
        Address = dto.Address ?? string.Empty,
        VenueType = dto.VenueType ?? string.Empty,
        ContactInfo = dto.ContactInfo ?? string.Empty,
        Capacity = dto.Capacity ?? 0,
        Bio = dto.Bio ?? string.Empty,
        Description = dto.Description ?? string.Empty,
        Amenities = dto.Amenities ?? string.Empty,
        OpeningHours = dto.OpeningHours ?? string.Empty,
        Genres = genres
      };
      return profile;
    }
  }

  public class SceneProfileQueryOptions
  {
    public bool IncludeGenres { get; set; }
    public bool IncludeMediaGallery { get; set; }

    public IQueryable<SceneProfile> Apply(IQueryable<SceneProfile> query)
    {
      if (IncludeGenres)
        query = query.Include(sp => sp.Genres);
      if (IncludeMediaGallery)
        query = query.Include(sp => sp.GalleryOwner);
      return query;
    }
  }
}
