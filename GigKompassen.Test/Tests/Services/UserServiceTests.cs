using GigKompassen.Data;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;

namespace GigKompassen.Test.Tests.Services
{
  public class UserServiceTests
  {
    private readonly UserService _userService;
    private readonly ApplicationDbContext _context;
    private readonly FakeDataHelper f;
    public UserServiceTests() 
    {
      _context = GetInMemoryDbContext();
      _userService = new UserService(_context, GetMockedUserManager(_context));
      MediaService mediaService = new MediaService(_context, _userService);
      GenreService genreService = new GenreService(_context);
      ArtistService artistService = new ArtistService(_context, _userService, genreService,  mediaService);
      SceneService sceneService = new SceneService(_context, _userService, genreService, mediaService);
      ManagerService managerService = new ManagerService(_context, _userService, mediaService);
      ProfileAccessService profileAccessService = new ProfileAccessService(_context, _userService, artistService, sceneService, managerService);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanGetUserById()
    {
      // Arrange
      f.SetupFakeData();
      var profile = f.AddFakeUser()!;
      // Act
      var result = await _userService.GetUserByIdAsync(profile.Id);
      // Assert
      Assert.Equal(profile, result);
    }

    [Fact]
    public async Task CanCompleteUserProfile()
    {
      var user = BogusRepositories.UserFaker.Generate();
      var user2 = BogusRepositories.UserFaker.Generate();
      
      user.ProfileCompleted = false;
      user2.ProfileCompleted = true;
      
      user2.FirstName = "oldFirstName";
      user2.LastName = "oldLastName";

      _context.Users.AddRange(user, user2);
      await _context.SaveChangesAsync();

      var result = await _userService.CompleteUserProfileAsync(user.Id, "firstName", "lastName");
      var result2 = await _userService.CompleteUserProfileAsync(user2.Id, "firstName", "lastName");
      var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
      var retrievedUser2 = await _context.Users.FirstOrDefaultAsync(u => u.Id == user2.Id);

      Assert.True(result);
      Assert.False(result2);
      
      Assert.True(retrievedUser.ProfileCompleted);
      Assert.True(retrievedUser2.ProfileCompleted);

      
      Assert.Equal("firstName", retrievedUser.FirstName);
      Assert.Equal("oldFirstName", retrievedUser2.FirstName);

      Assert.Equal("lastName", retrievedUser.LastName);
      Assert.Equal("oldLastName", retrievedUser2.LastName);
    }

    [Fact]
    public async Task CanDeleteUser()
    {
      f.SetupFakeData();

      var user = _context.Users.FirstOrDefault()!;

      var preretrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
      var preprofiles = _context.Profiles.Where(p => p.OwnerId == user.Id).ToList();
      var premediaGalleryOwnerIds = preprofiles.Select(p => p.MediaGalleryOwnerId).ToList();
      var premediaGalleries = _context.MediaGalleries.Where(m => premediaGalleryOwnerIds.Any(id => id == m.OwnerId)).ToList();
      var premediaItemIds = premediaGalleries.SelectMany(m => m.Items).Select(i => i.Id).ToList();
      var premediaLinkIds = _context.MediaItems.Where(m => premediaItemIds.Any(id => id == m.Id)).Select(i => i.MediaLinkId).ToList();
      var preownedMediaLinkIds = _context.MediaLinks.Where(l => l.UploaderId == user.Id).Select(l => l.Id).ToList();

      var result = await _userService.DeleteUserAsync(user.Id);

      var retrievedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
      var profiles = _context.Profiles.Where(p => p.OwnerId == user.Id).ToList();
      var mediaGalleryOwnerIds = profiles.Select(p => p.MediaGalleryOwnerId).ToList();
      var mediaGalleries = _context.MediaGalleries.Where(m => mediaGalleryOwnerIds.Contains(m.OwnerId)).ToList();
      var mediaItemIds = mediaGalleries.SelectMany(m => m.Items).Select(i => i.Id).ToList();
      var mediaLinkIds = _context.MediaItems.Where(m => mediaItemIds.Contains(m.Id)).Select(i => i.MediaLinkId).ToList();
      var ownedMediaLinkIds = _context.MediaLinks.Where(l => l.UploaderId == user.Id).Select(l => l.Id).ToList();

      Assert.True(result);
      Assert.Null(retrievedUser);
      Assert.Empty(profiles);
      Assert.Empty(mediaGalleries);
      Assert.Empty(mediaItemIds);
      Assert.Empty(mediaLinkIds);
      Assert.Empty(ownedMediaLinkIds);

    }

  }
}
