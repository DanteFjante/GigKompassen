using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;

namespace GigKompassen.Test.Tests.Services
{
  public class SceneServiceTests
  {

    private readonly SceneService _sceneService;
    private readonly ApplicationDbContext _context;
    private readonly GenreService _genreService;
    private readonly MediaService _mediaService;

    private readonly FakeDataHelper f;

    public SceneServiceTests() 
    {
      _context = GetInMemoryDbContext();
      _genreService = new GenreService(_context);
      _mediaService = new MediaService(_context);
      _sceneService = new SceneService(_context, _genreService, _mediaService);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanGetAll()
    {
      // Arrange
      f.SetupFakeData();
      var profile = f.AddFakeSceneProfile(Guid.NewGuid());

      // Act
      var result = await _sceneService.GetAllAsync();

      // Assert
      Assert.Equal(7, result.Count());
    }

    [Fact]
    public async Task CanGetById()
    {
      // Arrage
      f.SetupFakeData();
      var profile = f.AddFakeSceneProfile(Guid.NewGuid());
      // Act
      var result = await _sceneService.GetAsync(profile.Id);
      // Assert
      Assert.Equal(profile, result);
    }

    [Fact]
    public async Task CanCreate()
    {
      // Arrange
      var user = f.AddFakeUser();

      var sceneProfileDto = GetCreateSceneDtos(1).First();
      var genres = GetGenreNames(2).ToList();

      // Act
      var profile = await _sceneService.CreateAsync(user.Id, sceneProfileDto, genres);

      // Assert
      var allProfiles = _context.SceneProfiles.ToList();
      var allGenres = _context.Genres.ToList();

      Assert.Single(allProfiles);
      Assert.Equal(2, allGenres.Count());

      Assert.Equal(sceneProfileDto.Name, profile.Name);
      Assert.Equal(sceneProfileDto.Amenities, profile.Amenities);
      Assert.Equal(sceneProfileDto.Description, profile.Description);
      Assert.Equal(sceneProfileDto.Address, profile.Address);
      Assert.Equal(sceneProfileDto.Capacity, profile.Capacity);
      Assert.Equal(sceneProfileDto.ContactInfo, profile.ContactInfo);
      Assert.Equal(sceneProfileDto.VenueType, profile.VenueType);
      Assert.Equal(sceneProfileDto.Bio, profile.Bio);
      Assert.Equal(sceneProfileDto.OpeningHours, profile.OpeningHours);
      Assert.Equal(sceneProfileDto.PublicProfile, profile.Public);

      Assert.Equal(genres.Count(), profile.Genres!.Count());
      Assert.Contains(profile.Genres!, g => genres.Contains(g.Name));

      Assert.NotNull(profile.MediaGalleryOwner);
      Assert.NotNull(profile.MediaGalleryOwner.Galleries);
    }

    [Fact]
    public async Task CanUpdate()
    {
      // Arrange
      var sceneProfile = f.AddFakeSceneProfile(Guid.NewGuid());
      var genreToKeep = f.AddFakeGenreToScene(sceneProfile!.Id)!;
      var genreToDiscard = f.AddFakeGenreToScene(sceneProfile!.Id)!;
      var oldGenres = new List<Genre>() { genreToKeep, genreToDiscard };
      var newGenres = GetGenreNames(2).Append(genreToKeep.Name).ToList();

      var dto = new UpdateSceneDto(
        "Updated Name", 
        "Updated Address",
        "Updated VenueType",
        "Updated ContactInfo",
        200,
        "Updated Bio",
        "Updated Description",
        "Updated Amenities",
        "Updated OpeningHours",
        false
        );

      // Act
      var profile = await _sceneService.UpdateAsync(sceneProfile!.Id, dto, newGenres);

      // Assert
      var allProfiles = _context.SceneProfiles.ToList();
      var allGenres = _context.Genres.ToList();
      var genresOfProfile = _context.SceneProfiles.FirstOrDefault(ap => ap.Id == profile.Id)!.Genres;

      Assert.Single(allProfiles);
      Assert.Equal(3, allGenres.Count());
      Assert.Equal(3, genresOfProfile.Count());

      Assert.Equal(sceneProfile.Id, profile.Id);
      Assert.Equal("Updated Name", profile.Name);
      Assert.Equal("Updated Address", profile.Address);
      Assert.Equal("Updated VenueType", profile.VenueType);
      Assert.Equal("Updated ContactInfo", profile.ContactInfo);
      Assert.Equal(200, profile.Capacity);
      Assert.Equal("Updated Bio", profile.Bio);
      Assert.Equal("Updated Description", profile.Description);
      Assert.Equal("Updated Amenities", profile.Amenities);
      Assert.Equal("Updated OpeningHours", profile.OpeningHours);
      Assert.False(profile.Public);

      Assert.Contains(genresOfProfile, g => g.Name == genreToKeep.Name);
      Assert.DoesNotContain(genresOfProfile, g => g.Name == genreToDiscard.Name);
      Assert.DoesNotContain(allGenres, g => g.Name == genreToDiscard.Name);

    }

    [Fact]
    public async Task CanDelete()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var profile = f.AddFakeSceneProfile(user.Id)!;
      var genre1 = f.AddFakeGenreToScene(profile.Id)!;
      var genre2 = f.AddFakeGenreToScene(profile.Id)!;
      var gallery = f.AddFakeGallery(profile.MediaGalleryOwnerId)!;
      var item = f.AddFakeMediaItem(gallery.Id)!;
      var link = item.MediaLink!;

      var profileToKeep = f.AddFakeSceneProfile(user.Id)!;
      var genreToKeep = f.AddFakeGenreToScene(profileToKeep.Id)!;
      f.AddSceneToGenre(profileToKeep.Id, genreToKeep.Id);

      // Act
      await _sceneService.DeleteAsync(profile.Id);

      // Assert
      var allProfiles = _context.SceneProfiles.ToList();
      var allGenres = _context.Genres.ToList();
      var allGalleryOwners = _context.MediaGalleryOwners.ToList();
      var allGalleries = _context.MediaGalleries.ToList();
      var allItems = _context.MediaItems.ToList();
      var allLinks = _context.MediaLinks.ToList();

      Assert.Single(allProfiles);
      Assert.Equal(allProfiles.First(), profileToKeep);

      Assert.Single(allGenres);
      Assert.Equal(allGenres.First(), genreToKeep);

      Assert.Single(allGalleryOwners);
      Assert.Empty(allGalleries);
      Assert.Empty(allItems);
      Assert.Empty(allLinks);
    }

    [Fact]
    public async Task CanAddGenre()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var sceneProfile = f.AddFakeSceneProfile(user.Id)!;
      var genre = GetGenreNames(1).First();

      // Act
      await _sceneService.AddGenreAsync(sceneProfile.Id, genre);

      // Assert
      var profile = await _context.SceneProfiles.FirstOrDefaultAsync(p => p.Id == sceneProfile.Id);

      Assert.Contains(profile!.Genres!, g => g.Name == genre);
    }

    [Fact]
    public async Task CanRemoveGenre()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var sceneProfile = f.AddFakeSceneProfile(user.Id)!;
      var sceneProfile2 = f.AddFakeSceneProfile(user.Id)!;

      var genre = f.AddFakeGenreToScene(sceneProfile.Id)!;
      var genreToKeep = f.AddFakeGenreToScene(sceneProfile.Id)!;
      var genreToNotTouch = f.AddFakeGenreToScene(sceneProfile.Id)!;
      f.AddSceneToGenre(sceneProfile2.Id, genreToKeep.Id);

      // Act
      await _sceneService.RemoveGenreAsync(sceneProfile.Id, genre.Name);
      await _sceneService.RemoveGenreAsync(sceneProfile.Id, genreToKeep.Name);

      // Assert
      var profile = await _context.SceneProfiles.FirstOrDefaultAsync(p => p.Id == sceneProfile.Id);
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
