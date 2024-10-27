using GigKompassen.Data;
using GigKompassen.Dto.Profiles;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class ArtistService
  {
    private readonly ApplicationDbContext _context;
    private readonly MediaService _mediaService;
    private readonly GenreService _genreService;


    public ArtistService(ApplicationDbContext context, MediaService mediaService, GenreService genreService)
    {
      _context = context;
      _mediaService = mediaService;
      _genreService = genreService;
    }

    public async Task<List<ArtistProfile>> GetAllAsync(ArtistProfileQueryOptions? options = null)
    {

      var query = _context.ArtistProfiles.IgnoreAutoIncludes().AsQueryable();

      if (options != null)
      {
        query = options.Apply(query);
      }

      var artistProfiles = await query.ToListAsync();

      return artistProfiles;
    }


    public async Task<ArtistProfile?> GetAsync(Guid id, ArtistProfileQueryOptions? options = null)
    {
      var query = _context.ArtistProfiles.AsQueryable();

      if (options != null)
      {
        query = options.Apply(query);
      }

      var artistProfile = await query.FirstOrDefaultAsync(ap => ap.Id == id);

      if (artistProfile == null)
        return null;

      // Map domain model to DTO
      return artistProfile;
    }

    public async Task<ArtistProfile> CreateAsync(Guid userId, ArtistProfileDto artistProfileDto)
    {
      if (artistProfileDto == null)
        throw new ArgumentNullException(nameof(artistProfileDto));

      if (string.IsNullOrWhiteSpace(artistProfileDto.Name))
        throw new ArgumentException("Name is required.", nameof(artistProfileDto.Name));

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

      if (user == null)
        throw new KeyNotFoundException("User not found");

      var profile = await FromDtoAsync(artistProfileDto);
      profile.OwnerId = userId;
      profile.Owner = user;
      await _context.ArtistProfiles.AddAsync(profile);
      user.OwnedProfiles?.Add(profile);
      await _context.SaveChangesAsync();
      var galleryOwner = await _mediaService.CreateGalleryOwnerAsync(profile);
      profile.GalleryOwner = galleryOwner;
      return profile;
    }

    public async Task<ArtistProfile> UpdateAsync(Guid id, ArtistProfileDto artistProfileDto)
    {
      if (artistProfileDto == null)
        throw new ArgumentNullException(nameof(artistProfileDto));

      var profile = await _context.ArtistProfiles
          .Include(ap => ap.Genres)
          .Include(ap => ap.Members)
          .AsTracking()
          .FirstOrDefaultAsync(ap => ap.Id == id);

      if (profile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      // Update properties
      profile.Name = artistProfileDto.Name ?? profile.Name;
      profile.Location = artistProfileDto.Location ?? profile.Location;
      profile.Description = artistProfileDto.Description ?? profile.Description;
      profile.Availability = artistProfileDto.Availability ?? profile.Availability;
      profile.Bio = artistProfileDto.Bio ?? profile.Bio;

      // Update Genres
      if (artistProfileDto.Genres != null)
      {
        var genreNames = artistProfileDto.Genres.Select(g => g.Name).ToList();

        var genresToRemove = profile.Genres.Where(g => !genreNames.Contains(g.Name)).ToList();
        var genresToAdd = await _genreService.AddOrGetGenresAsync(genreNames.Except(profile.Genres.Select(p => p.Name)));
        
        
        foreach(var genre in genresToRemove)
        {
          profile.Genres.Remove(genre);
        }
        
        foreach (var genre in genresToAdd)
        {
          profile.Genres.Add(genre);
        }
      }

      // Update Members
      if (artistProfileDto.Members != null)
      {
        var toAdd = artistProfileDto.Members
          .Where(m => !profile.Members.Any(p => p.Name == m.Name && p.Role == m.Role))
          .Select(m => new ArtistMember
          {
            Id = m.Id.HasValue ? m.Id.Value : Guid.NewGuid(),
            Name = m.Name!,
            Role = m.Role!
          })
          .ToList();

        var toRemove = profile.Members
          .Where(m => !artistProfileDto.Members.Any(p => p.Name == m.Name && p.Role == m.Role))
          .ToList();

        foreach (var member in toAdd)
        {
          profile.Members.Add(member);
          _context.ArtistMembers.Add(member);
        }

        
        foreach (var member in toRemove)
        {
          profile.Members.Remove(member);
          _context.ArtistMembers.Remove(member);
        }
        
      }

      await _context.SaveChangesAsync();

      return profile;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
      var artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.GalleryOwner)
          .FirstOrDefaultAsync(ap => ap.Id == id);

      if (artistProfile != null)
      {
        if (artistProfile.GalleryOwner != null)
        {
          if (artistProfile.GalleryOwner.Gallery != null)
            _context.MediaGalleries.Remove(artistProfile.GalleryOwner.Gallery);
          _context.MediaGalleryOwners.Remove(artistProfile.GalleryOwner);
        }
        _context.ArtistMembers.RemoveRange(artistProfile.Members);
        _context.ArtistProfiles.Remove(artistProfile);
        await _context.SaveChangesAsync();
        return true;
      }
      else
      {
        return false;
      }
    }

    #region ArtistMembers
    public async Task<ArtistMember> AddMember(Guid artistProfileId, ArtistMemberDto member)
    {
      ArtistMember newMember = new ArtistMember()
      {
        Id = Guid.NewGuid(),
        Name = member.Name!,
        Role = member.Role!,
        ArtistProfileId = artistProfileId,
      };
      await _context.ArtistMembers.AddAsync(newMember);
      await _context.SaveChangesAsync();
      return newMember;
    }

    public async Task<bool> RemoveMember(Guid memberId)
    {
      var member = await _context.ArtistMembers.FirstOrDefaultAsync(m => m.Id == memberId);
      if (member == null)
        return false;
      _context.ArtistMembers.Remove(member);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> UpdateMember(Guid memberId, ArtistMemberDto member)
    {
      var existingMember = await _context.ArtistMembers.FirstOrDefaultAsync(m => m.Id == memberId);
      if (existingMember == null)
        return false;

      if (member.Name != null)
        existingMember.Name = member.Name;

      if (member.Role != null)
        existingMember.Role = member.Role;

      await _context.SaveChangesAsync();
      return true;
    }
    #endregion

    private async Task<ArtistProfile> FromDtoAsync(ArtistProfileDto dto)
    {

      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      if (string.IsNullOrWhiteSpace(dto.Name))
        throw new ArgumentException("Name is required.", nameof(dto.Name));

      if (!dto.Availability.HasValue)
        throw new ArgumentException("Availability is required.", nameof(dto.Availability));

      Guid artistId = dto.Id.HasValue ? dto.Id.Value : Guid.NewGuid();

      List<Genre> genres = new List<Genre>();
      if (dto.Genres != null && dto.Genres.Any())
      {
        var genreNames = dto.Genres.Select(g => g.Name).ToList();
        genres = await _genreService.AddOrGetGenresAsync(genreNames);
      }

      List<ArtistMember> members = dto.Members?
        .Where(m => !string.IsNullOrWhiteSpace(m.Name) && !string.IsNullOrWhiteSpace(m.Role))
        .Select(m => new ArtistMember
        {
          Id = m.Id.HasValue ? m.Id.Value : Guid.NewGuid(),
          Name = m.Name!,
          Role = m.Role!,
          ArtistProfileId = artistId
        })
        .ToList() ?? new List<ArtistMember>();

      var profile = new ArtistProfile
      {
        Id = artistId,
        Name = dto.Name!,
        Location = dto.Location ?? string.Empty,
        Availability = dto.Availability.Value,
        Bio = dto.Bio ?? string.Empty,
        Description = dto.Description ?? string.Empty,
        Members = members,
        Genres = genres
      };
      return profile;
    }
  }

  public class ArtistProfileQueryOptions
  {
    public bool includeGenres { get; set; }
    public bool includeMembers { get; set; }
    public bool includeGalleryOwner { get; set; }

    public IQueryable<ArtistProfile> Apply(IQueryable<ArtistProfile> query)
    {
      if (includeGenres)
        query = query.Include(ap => ap.Genres);
      if (includeMembers)
        query = query.Include(ap => ap.Members);
      if (includeGalleryOwner)
        query = query.Include(ap => ap.GalleryOwner);
      return query;
    }
  }
}