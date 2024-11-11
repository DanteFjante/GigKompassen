using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;

namespace GigKompassen.Services
{
  public class ArtistService
  {

    public delegate void GenreEventHandler(object? sender, ArtistProfile artistProfile, Genre genre);
    public delegate void GenresEventHandler(object? sender, ArtistProfile artistProfile, List<Genre> genres);

    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;

    // Events
    public event AsyncEventHandler<ArtistProfile> OnCreateArtistProfile;
    public event AsyncEventHandler<ArtistProfile> OnUpdateArtistProfile;
    public event AsyncEventHandler<ArtistProfile> OnDeleteArtistProfile;
    public event AsyncEventHandler<ArtistMember> OnAddArtistMember;
    public event AsyncEventHandler<ArtistMember> OnUpdateArtistMember;
    public event AsyncEventHandler<ArtistMember> OnRemoveArtistMember;
    public event AsyncEventHandler<GenreEventArgs> OnAddGenre;
    public event AsyncEventHandler<GenresEventArgs> OnAddGenres;
    public event AsyncEventHandler<GenreEventArgs> OnRemoveGenre;
    public event AsyncEventHandler<GenresEventArgs> OnRemoveGenres;


    public ArtistService(ApplicationDbContext context, UserService userService, GenreService genreService, MediaService mediaService)
    {
      _context = context;
      _userService = userService;
      _genreService = genreService;
      _mediaService = mediaService;

      if (_userService != null)
      {
        _userService.OnDeleteUser += async (sender, user) =>
        {
          var profiles = await GetArtistProfilesOwnerByUserAsync(user.Id);
          await Task.WhenAll(profiles.Select(p => DeleteAsync(p.Id)));
        };
      }

    }

    #region Getters
    public async Task<List<ArtistProfile>> GetAllAsync()
    {
      var artistProfiles = await _context.ArtistProfiles.ToListAsync();

      return artistProfiles;
    }


    public async Task<ArtistProfile?> GetAsync(Guid id)
    {
      var artistProfile = await _context.ArtistProfiles.FirstOrDefaultAsync(ap => ap.Id == id);

      if (artistProfile == null)
        return null;

      return artistProfile;
    }

    public async Task<List<ArtistProfile>> GetArtistProfilesUsingGenre(string genreName)
    {
      var genre = await _genreService.GetGenreAsync(genreName);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetArtistProfilesUsingGenre(genre);
    }

    public async Task<List<ArtistProfile>> GetArtistProfilesUsingGenre(Guid genreId)
    {
      var genre = await _genreService.GetGenreAsync(genreId);
      if (genre == null)
        throw new KeyNotFoundException("Genre not found");

      return await GetArtistProfilesUsingGenre(genre);
    }

    public async Task<List<ArtistProfile>> GetArtistProfilesUsingGenre(Genre genre)
    {
      return await _context.ArtistProfiles
                .Include(ap => ap.Genres)
                .Where(ap => ap.Genres.Contains(genre))
                .ToListAsync();
    }

    public async Task<List<ArtistProfile>> GetArtistProfilesOwnerByUserAsync(Guid userId)
    {
      if (!await _context.Users.AnyAsync(p => p.Id == userId))
        throw new KeyNotFoundException("User not found");

      return await _context.ArtistProfiles.Where(p => p.OwnerId == userId).ToListAsync();
    }
    #endregion

    #region Creators
    public async Task<ArtistProfile> CreateAsync(Guid applicationUserId, CreateArtistDto artistProfileDto, List<string>? genres = null, List<ArtistMemberDto>? members = null)
    {
      if (artistProfileDto == null)
        throw new ArgumentNullException(nameof(artistProfileDto));

      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == applicationUserId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      ArtistProfile profile = artistProfileDto.ToArtistProfile();
      profile.OwnerId = user.Id;
      profile.Owner = user;

      if (genres != null && genres.Any())
      {
        var genreList = await _genreService.GetOrCreateGenresAsync(genres);
        profile.Genres!.AddRange(genreList);
      }

      if (members != null && members.Any())
      {
        profile.Members!.AddRange(members.Select(m => m.ToArtistMember(profile)));
      }

      MediaGalleryOwner mediaGalleryOwner = new MediaGalleryOwner()
      {
        Id = Guid.NewGuid(),
        Galleries = new List<MediaGallery>(),
      };

      profile.MediaGalleryOwnerId = mediaGalleryOwner.Id;
      profile.MediaGalleryOwner = mediaGalleryOwner;

      if(OnCreateArtistProfile != null)
        await OnCreateArtistProfile.InvokeAsync(this, profile);

      await _context.ArtistProfiles.AddAsync(profile);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create ArtistProfile");

      return profile;
    }
    #endregion

    #region Updaters
    public async Task<ArtistProfile> UpdateAsync(Guid id, UpdateArtistDto artistProfileDto, List<string>? genreNames = null, List<ArtistMemberDto>? members = null)
    {

      if (artistProfileDto == null)
        throw new ArgumentNullException(nameof(artistProfileDto));

      ArtistProfile? profile = await _context.ArtistProfiles.AsTracking().Include(p => p.Members).Include(p => p.Genres).FirstOrDefaultAsync(p => p.Id == id);
      if(profile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      artistProfileDto.UpdateProfile(profile);

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

      if (members != null && members.Any())
      {
        var newMemberIds = members.Select(m => m.Id).ToList();
        List<Guid> idsToRemove = new();
        List<ArtistMemberDto> membersToUpdate = new();
        List<ArtistMemberDto> membersToCreate = new();
        if (profile.Members != null)
        {
          var currentIds = profile.Members.Select(m => m.Id).ToList();
          idsToRemove = currentIds.SkipWhile(id => newMemberIds.Contains(id)).ToList();

          membersToUpdate = members.Where(m => m.Id.HasValue && currentIds.Contains(m.Id.Value)).ToList();
        }
        membersToCreate = members.Where(m => !membersToUpdate.Contains(m)).ToList();

        foreach (var memberId in idsToRemove)
        {
          await RemoveMemberAsync(memberId);
        }
        foreach (var member in membersToUpdate)
        {
          await UpdateMemberAsync(member);
        }
        foreach (var member in membersToCreate)
        {
          await AddMemberAsync(profile.Id, member);
        }
      }

      if(OnUpdateArtistProfile != null)
        await OnUpdateArtistProfile.InvokeAsync(this, profile);

      _context.ArtistProfiles.Update(profile);
      var res = await _context.SaveChangesAsync();
      if(res == 0)
        throw new DbUpdateException("Failed to update ArtistProfile");

      return profile;
    }
    #endregion

    #region Deleters
    public async Task<bool> DeleteAsync(Guid id)
    {
      var artistProfile = await _context.ArtistProfiles
          .FirstOrDefaultAsync(ap => ap.Id == id);

      if (artistProfile == null)
        return false;



      if(artistProfile.Members != null && artistProfile.Members.Any())
        _context.ArtistMembers.RemoveRange(artistProfile.Members);

      if(artistProfile.Genres != null && artistProfile.Genres.Any())
        await RemoveGenresAsync(artistProfile.Id, artistProfile.Genres.Select(g => g.Name).ToList());

      if (OnDeleteArtistProfile != null)
        await OnDeleteArtistProfile.InvokeAsync(this, artistProfile);


      _context.ArtistProfiles.Remove(artistProfile);

      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete ArtistProfile");

      if (artistProfile.MediaGalleryOwner != null)
      {
        await _mediaService.DeleteMediaGalleryOwnerAsync(artistProfile.MediaGalleryOwner.Id);
      }

      return true;
    }
    #endregion

    #region ArtistMembers
    public async Task<ArtistMember> AddMemberAsync(Guid artistProfileId, ArtistMemberDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      ArtistProfile? artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.Members)
          .FirstOrDefaultAsync(ap => ap.Id == artistProfileId);
      if(artistProfile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      ArtistMember newMember = dto.ToArtistMember(artistProfile);

      if(OnAddArtistMember != null)
        await OnAddArtistMember.InvokeAsync(this, newMember);

      await _context.ArtistMembers.AddAsync(newMember);

      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add ArtistMember");

      return newMember;
    }

    public async Task<bool> UpdateMemberAsync(ArtistMemberDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      if(!dto.Id.HasValue)
        throw new ArgumentException("Id is required.", nameof(dto.Id));

      var existingMember = await _context.ArtistMembers.FirstOrDefaultAsync(m => m.Id == dto.Id);
      if (existingMember == null)
        return false;

      dto.UpdateArtistMember(existingMember);

      if(OnUpdateArtistMember != null)
        await OnUpdateArtistMember.InvokeAsync(this, existingMember);
      
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to update ArtistMember");

      return true;
    }

    public async Task<bool> RemoveMemberAsync(Guid memberId)
    {
      var member = await _context.ArtistMembers.FirstOrDefaultAsync(m => m.Id == memberId);
      if (member == null)
        return false;

      if(OnRemoveArtistMember != null)
        await OnRemoveArtistMember.InvokeAsync(this, member);
      
      _context.ArtistMembers.Remove(member);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to remove ArtistMember");

      return true;
    }


    #endregion

    #region Genres
    public async Task<Genre> AddGenreAsync(Guid artistProfileId, string genreName)
    {
      var artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == artistProfileId);

      if (artistProfile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      var genre = await _genreService.GetOrCreateGenreAsync(genreName);

      if(OnAddGenre != null)
        await OnAddGenre.InvokeAsync(this, new(artistProfile, genre));

      artistProfile.Genres!.Add(genre);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add Genre");

      return genre;
    }

    public async Task<List<Genre>> AddGenresAsync(Guid artistProfileId, List<string> genreNames)
    {
      var artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == artistProfileId);

      if (artistProfile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      var genres = await _genreService.GetOrCreateGenresAsync(genreNames);
      
      if(OnRemoveGenre != null)
        await OnAddGenres.InvokeAsync(this, new(artistProfile, genres));

      artistProfile.Genres!.AddRange(genres);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to add Genres");

      return genres;
    }

    public async Task<bool> RemoveGenreAsync(Guid artistProfileId, string genreName)
    {
      var artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == artistProfileId);

      if (artistProfile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      var genre = await _genreService.GetGenreAsync(genreName);

      if (genre == null || artistProfile.Genres == null || !artistProfile.Genres.Any())
        return false;

      if (artistProfile.Genres.Contains(genre))
      {
        if(OnRemoveGenre != null)
          await OnRemoveGenre.InvokeAsync(this, new(artistProfile, genre));
        
        artistProfile.Genres.Remove(genre);
        if(await _context.SaveChangesAsync() == 0)
          throw new DbUpdateException("Failed to remove Genre");
      }
      else
        return false;

      var artistProfilesUsingGenre = await GetArtistProfilesUsingGenre(genre);

      if (!artistProfilesUsingGenre.Any())
      {
        await _genreService.RemoveGenreAsync(genre);
      }


      return true;
    }

    public async Task<int> RemoveGenresAsync(Guid artistProfileId, List<string> genreNames)
    {
      var artistProfile = await _context.ArtistProfiles
          .Include(ap => ap.Genres)
          .FirstOrDefaultAsync(ap => ap.Id == artistProfileId);
      if (artistProfile == null)
        throw new KeyNotFoundException("ArtistProfile not found");

      if(artistProfile.Genres == null || !artistProfile.Genres.Any())
        return 0;

      var toRemove = artistProfile.Genres.Where(g => genreNames.Contains(g.Name)).ToList();

      if(OnRemoveGenres != null)
        await OnRemoveGenres.InvokeAsync(this, new (artistProfile, toRemove));

      foreach (var genre in toRemove)
      {
        artistProfile.Genres.Remove(genre);
      }
      int result = await _context.SaveChangesAsync();

      foreach (var genre in toRemove)
      {
        var artistProfilesUsingGenre = await GetArtistProfilesUsingGenre(genre);

        if (!artistProfilesUsingGenre.Any())
        {
          await _genreService.RemoveGenreAsync(genre);
        }
      }
      
      return result;
    }
    #endregion

    public record class GenreEventArgs(ArtistProfile ArtistProfile, Genre Genre);
    public record class GenresEventArgs(ArtistProfile ArtistProfile, List<Genre> Genres);
  }
  
  public record class ArtistMemberDto(Guid? Id = null, string? Name = null, string? Role = null)
  {
    public ArtistMember ToArtistMember(ArtistProfile artistProfile)
    {
      if(string.IsNullOrWhiteSpace(Name))
        throw new ArgumentException("Name is required.", nameof(Name));

      if(string.IsNullOrWhiteSpace(Role))
        throw new ArgumentException("Role is required.", nameof(Role));

      Guid id = Id.HasValue ? Id.Value : Guid.NewGuid();

      return new ArtistMember()
      {
        Id = id,
        Name = Name,
        Role = Role,
        ArtistProfileId = artistProfile.Id,
        ArtistProfile = artistProfile,
      };
    }

    public void UpdateArtistMember(ArtistMember member)
    {
      if (Name != null)
        member.Name = Name;

      if (Role != null)
        member.Role = Role;
    }

    public static ArtistMemberDto FromArtistMember(ArtistMember artistMember)
    {
      return new ArtistMemberDto(artistMember.Id, artistMember.Name, artistMember.Role);
    }
  }
  public record class CreateArtistDto(string Name, string? Location, string? Bio, string? Description, AvailabilityStatus Availability, bool PublicProfile = true)
  {
    public ArtistProfile ToArtistProfile()
    {
      if(string.IsNullOrWhiteSpace(Name))
        throw new ArgumentException("Name is required.", nameof(Name));

      Guid id = Guid.NewGuid();

      ArtistProfile profile =  new ArtistProfile()
      {
        Id = Guid.NewGuid(),
        Name = Name,
        Location = Location ?? string.Empty,
        Bio = Bio ?? string.Empty,
        Description = Description ?? string.Empty,
        Availability = Availability,
        Public = PublicProfile,
        Members = new List<ArtistMember>(),
        Genres = new List<Genre>(),
      };

      return profile;
    }
    public static CreateArtistDto FromArtistProfile(ArtistProfile artistProfile)
    {
      return new CreateArtistDto(artistProfile.Name, artistProfile.Location, artistProfile.Bio, artistProfile.Description, artistProfile.Availability, artistProfile.Public);
    }
  }
  public record class UpdateArtistDto(string? Name = null, string? Location = null, string? Bio = null, string? Description = null, AvailabilityStatus? Availability = null, bool? PublicProfile = null)
  {
    public void UpdateProfile(ArtistProfile profile)
    { 
      if(Name != null)
        profile.Name = Name;

      if (Location != null)
        profile.Location = Location;

      if (Bio != null)
        profile.Bio = Bio;

      if (Description != null)
        profile.Description = Description;

      if (Availability != null)
        profile.Availability = Availability.Value;

      if (PublicProfile != null)
        profile.Public = PublicProfile.Value;
    }
    public static UpdateArtistDto FromArtistProfile(ArtistProfile artistProfile)
    {
      return new UpdateArtistDto(artistProfile.Name, artistProfile.Location, artistProfile.Bio, artistProfile.Description, artistProfile.Availability, artistProfile.Public);
    }
  }
}