using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;
using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Test.Tests.Services
{
  public class ArtistServiceTests
  {
    private readonly ArtistService _artistService;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;
    private readonly ApplicationDbContext _context;

    private readonly FakeDataHelper f;

    public ArtistServiceTests() 
    {
      _context = GetInMemoryDbContext();
      _genreService = new GenreService(_context);
      _mediaService = new MediaService(_context);
      _artistService = new ArtistService(_context, _genreService, _mediaService);
      
      f = new FakeDataHelper(_context);
    }

    #region Fake Data setup
    #endregion

    [Fact]
    public async Task CanGetAllArtistProfiles()
    {
      // Arrange
      f.SetupFakeData();
      var profile = f.AddFakeArtistProfile(Guid.NewGuid());

      // Act
      var result = await _artistService.GetAllAsync();

      // Assert
      Assert.Equal(7, result.Count());
    }

    [Fact]
    public async Task CanGetArtistProfileById()
    {
      // Arrage
      f.SetupFakeData();
      var profile = f.AddFakeArtistProfile(Guid.NewGuid());
      // Act
      var result = await _artistService.GetAsync(profile.Id);
      // Assert
      Assert.Equal(profile, result);
    }

    [Fact]
    public async Task CanCreateArtistProfile()
    {
      // Arrange
      var user = f.AddFakeUser();

      var artistProfileDto = GetCreateArtistDtos(1).First();
      var genres = GetGenreNames(2).ToList();
      var members = GetArtistMemberDtos(2).ToList();

      // Act
      var profile = await _artistService.CreateAsync(user.Id, artistProfileDto, genres, members);
      
      // Assert
      var allProfiles = _context.ArtistProfiles.ToList();
      var allMembers = _context.ArtistMembers.ToList();
      var allGenres = _context.Genres.ToList();

      Assert.Single(allProfiles);
      Assert.Equal(2, allMembers.Count());
      Assert.Equal(2, allGenres.Count());

      Assert.Equal(artistProfileDto.Name, profile.Name);
      Assert.Equal(artistProfileDto.PublicProfile, profile.Public);
      Assert.Equal(artistProfileDto.Location, profile.Location);
      Assert.Equal(artistProfileDto.Bio, profile.Bio);
      Assert.Equal(artistProfileDto.Description, profile.Description);
      Assert.Equal(artistProfileDto.Availability, profile.Availability);

      Assert.Equal(genres.Count(), profile.Genres!.Count());
      Assert.Contains(profile.Genres!, g => genres.Contains(g.Name));

      Assert.Equal(members.Count(), profile.Members!.Count());
      Assert.Contains(profile.Members!, m => members.Select(am => am.Name).Contains(m.Name));
      Assert.Contains(profile.Members!, m => members.Select(am => am.Role).Contains(m.Role));

      Assert.NotNull(profile.MediaGalleryOwner);
      Assert.NotNull(profile.MediaGalleryOwner.Galleries);
    }

    [Fact]
    public async Task CanUpdateArtistProfile()
    {
      // Arrange
      var artistProfile = f.AddFakeArtistProfile(Guid.NewGuid());
      var genreToKeep = f.AddFakeGenreToArtist(artistProfile!.Id)!;
      var genreToDiscard = f.AddFakeGenreToArtist(artistProfile!.Id)!;
      var oldGenres = new List<Genre>() {genreToKeep, genreToDiscard};
      var newGenres = GetGenreNames(2).Append(genreToKeep.Name).ToList();

      var memberToKeep = f.AddFakeArtistMember(artistProfile!.Id)!;
      var updatedMemberDto = new ArtistMemberDto(memberToKeep.Id, "Updated Name", "Updated Role");
      var memberToDiscard = f.AddFakeArtistMember(artistProfile!.Id)!;
      var oldMembers = new List<ArtistMember>() { memberToKeep, memberToDiscard };
      var newMembers = GetArtistMemberDtos(2).Append(updatedMemberDto).ToList();

      var dto = new UpdateArtistDto("Updated Name", "Updated Location", "Updated Bio", "Updated Description", AvailabilityStatus.Closed, false);

      // Act
      var profile = await _artistService.UpdateAsync(artistProfile!.Id, dto, newGenres, newMembers);
      
      // Assert
      var allProfiles = _context.ArtistProfiles.ToList();
      var allMembers = _context.ArtistMembers.ToList();
      var allGenres = _context.Genres.ToList();
      var genresOfProfile = _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == profile.Id)!.Genres;
      var membersOfProfile = _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == profile.Id)!.Members;
      var updatedMember = _context.ArtistMembers.FirstOrDefault(m => m.Id == memberToKeep.Id);

      Assert.Single(allProfiles);
      Assert.Equal(3, allMembers.Count());
      Assert.Equal(3, membersOfProfile.Count());
      Assert.Equal(3, allGenres.Count());
      Assert.Equal(3, genresOfProfile.Count());

      Assert.Equal(artistProfile.Id, profile.Id); 
      Assert.Equal("Updated Name", profile.Name);
      Assert.Equal("Updated Location", profile.Location);
      Assert.Equal("Updated Bio", profile.Bio);
      Assert.Equal("Updated Description", profile.Description);
      Assert.Equal(AvailabilityStatus.Closed, profile.Availability);
      Assert.False(profile.Public);

      Assert.Contains(membersOfProfile, m => m.Id == memberToKeep.Id);
      Assert.DoesNotContain(membersOfProfile, m => m.Id == memberToDiscard.Id);
      Assert.DoesNotContain(allMembers, m => m.Id == memberToDiscard.Id);
      
      Assert.Contains(membersOfProfile, m => m.Name == "Updated Name");
      Assert.Contains(membersOfProfile, m => m.Role == "Updated Role");

      Assert.Contains(genresOfProfile, g => g.Name == genreToKeep.Name);
      Assert.DoesNotContain(genresOfProfile, g => g.Name == genreToDiscard.Name);
      Assert.DoesNotContain(allGenres, g => g.Name == genreToDiscard.Name);

    }

    [Fact]
    public async Task CanDeleteArtistProfile()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var profile = f.AddFakeArtistProfile(user.Id)!;
      var member1 = f.AddFakeArtistMember(profile.Id)!;
      var member2 = f.AddFakeArtistMember(profile.Id)!;
      var genre1 = f.AddFakeGenreToArtist(profile.Id)!;
      var genre2 = f.AddFakeGenreToArtist(profile.Id)!;
      var gallery = f.AddFakeGallery(profile.MediaGalleryOwnerId)!;
      var item = f.AddFakeMediaItem(gallery.Id)!;
      var link = item.MediaLink!;

      var profileToKeep = f.AddFakeArtistProfile(user.Id)!;
      var genreToKeep = f.AddFakeGenreToArtist(profileToKeep.Id)!;
      f.AddArtistToGenre(profileToKeep.Id, genreToKeep.Id);

      // Act
      await _artistService.DeleteAsync(profile.Id);

      // Assert
      var allProfiles = _context.ArtistProfiles.ToList();
      var allMembers = _context.ArtistMembers.ToList();
      var allGenres = _context.Genres.ToList();
      var allGalleryOwners = _context.MediaGalleryOwners.ToList();
      var allGalleries = _context.MediaGalleries.ToList();
      var allItems = _context.MediaItems.ToList();
      var allLinks = _context.MediaLinks.ToList();

      Assert.Single(allProfiles);
      Assert.Equal(allProfiles.First(), profileToKeep);
      
      Assert.Empty(allMembers);
      Assert.Single(allGenres);
      Assert.Equal(allGenres.First(), genreToKeep);

      Assert.Single(allGalleryOwners);
      Assert.Empty(allGalleries);
      Assert.Empty(allItems);
      Assert.Empty(allLinks);

    }

    [Fact]
    public async Task CanAddArtistMember()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var artistProfile = f.AddFakeArtistProfile(user.Id)!;
      var dto = GetArtistMemberDtos(1).First();

      // Act
      await _artistService.AddMemberAsync(artistProfile.Id, dto);

      // Assert
      var profile = await _context.ArtistProfiles.FirstOrDefaultAsync(p => p.Id == artistProfile.Id);

      Assert.Contains(profile.Members, m => m.Name == dto.Name);
      Assert.Contains(profile.Members, m => m.Role == dto.Role);
    }

    [Fact]
    public async Task CanUpdateArtistMember()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var artistProfile = f.AddFakeArtistProfile(user.Id)!;
      var member = f.AddFakeArtistMember(artistProfile.Id)!;

      var updated = ArtistMemberFaker.Generate();
      updated.Id = member.Id;
      var dto = ArtistMemberDto.FromArtistMember(updated);

      // Act
      await _artistService.UpdateMemberAsync(dto);
      
      // Assert
      var profile = await _context.ArtistProfiles.FirstOrDefaultAsync(p => p.Id == artistProfile.Id);

      Assert.Contains(profile.Members, m => m.Name == updated.Name);
      Assert.Contains(profile.Members, m => m.Role == updated.Role);
    }

    [Fact]
    public async Task CanRemoveArtistMember()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var artistProfile = f.AddFakeArtistProfile(user.Id)!;
      var member = f.AddFakeArtistMember(artistProfile.Id)!;

      // Act
      await _artistService.RemoveMemberAsync(member.Id);
      
      // Assert
      var profile = await _context.ArtistProfiles.FirstOrDefaultAsync(p => p.Id == artistProfile.Id);
      var members = await _context.ArtistMembers.ToListAsync();

      Assert.DoesNotContain(profile!.Members!, m => m.Id == member.Id);
      Assert.DoesNotContain(members, m => m.Id == member.Id);
    }

    [Fact]
    public async Task CanAddGenre()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var artistProfile = f.AddFakeArtistProfile(user.Id)!;
      var genre = GetGenreNames(1).First();
      
      // Act
      await _artistService.AddGenreAsync(artistProfile.Id, genre);

      // Assert
      var profile = await _context.ArtistProfiles.FirstOrDefaultAsync(p => p.Id == artistProfile.Id);

      Assert.Contains(profile!.Genres!, g => g.Name == genre);
    }

    [Fact]
    public async Task CanRemoveGenre()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var artistProfile = f.AddFakeArtistProfile(user.Id)!;
      var artistProfile2 = f.AddFakeArtistProfile(user.Id)!;

      var genre = f.AddFakeGenreToArtist(artistProfile.Id)!;
      var genreToKeep = f.AddFakeGenreToArtist(artistProfile.Id)!;
      var genreToNotTouch = f.AddFakeGenreToArtist(artistProfile.Id)!;
      f.AddArtistToGenre(artistProfile2.Id, genreToKeep.Id);
      
      // Act
      await _artistService.RemoveGenreAsync(artistProfile.Id, genre.Name);
      await _artistService.RemoveGenreAsync(artistProfile.Id, genreToKeep.Name);

      // Assert
      var profile = await _context.ArtistProfiles.FirstOrDefaultAsync(p => p.Id == artistProfile.Id);
      var genres = await _context.Genres.ToListAsync();
      
      Assert.Equal(2, genres.Count);
      Assert.Single(profile!.Genres!);

      Assert.Contains(genres!, g => g.Name == genreToKeep.Name);
      Assert.Contains(genres!, g => g.Name == genreToNotTouch.Name);
      Assert.DoesNotContain(genres!, g => g.Name == genre.Name);

      Assert.Contains(profile!.Genres!, g => g.Name == genreToNotTouch.Name);
      Assert.DoesNotContain(profile!.Genres!, g => g.Name == genre.Name);
      Assert.DoesNotContain(profile!.Genres!, g => g.Name == genreToKeep.Name);
    }
  }
}