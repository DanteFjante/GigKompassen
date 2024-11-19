using GigKompassen.Data;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;

namespace GigKompassen.Test.Tests.Services
{
  public class ManagerServiceTests
  {
    private readonly ManagerService _managerService;
    private readonly ApplicationDbContext _context;
    private readonly MediaService _mediaService;

    private readonly FakeDataHelper f;

    public ManagerServiceTests() 
    {
      _context = GetInMemoryDbContext();
      _mediaService = new MediaService(_context);
      _managerService = new ManagerService(_context, _mediaService);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanGetAll()
    {
      // Arrange
      f.SetupFakeData();
      var profile = f.AddFakeManagerProfile(Guid.NewGuid());

      // Act
      var result = await _managerService.GetAllAsync();

      // Assert
      Assert.Equal(7, result.Count());
    }

    [Fact]
    public async Task CanGetById()
    {
      // Arrage
      f.SetupFakeData();
      var profile = f.AddFakeManagerProfile(Guid.NewGuid());
      // Act
      var result = await _managerService.GetAsync(profile.Id);
      // Assert
      Assert.Equal(profile, result);
    }

    [Fact]
    public async Task CanCreate()
    {
      // Arrange
      var user = f.AddFakeUser();

      var managerProfileDto = GetCreateManagerDtos(1).First();

      // Act
      var profile = await _managerService.CreateAsync(user.Id, managerProfileDto);

      // Assert
      var allProfiles = _context.ManagerProfiles.ToList();

      Assert.Single(allProfiles);

      Assert.Equal(managerProfileDto.Name, profile.Name);
      Assert.Equal(managerProfileDto.Description, profile.Description);
      Assert.Equal(managerProfileDto.Location, profile.Location);
      Assert.Equal(managerProfileDto.PublicProfile, profile.Public);

      Assert.NotNull(profile.MediaGalleryOwner);
      Assert.NotNull(profile.MediaGalleryOwner.Galleries);
    }

    [Fact]
    public async Task CanUpdate()
    {
      // Arrange
      var managerProfile = f.AddFakeManagerProfile(Guid.NewGuid());

      var dto = new UpdateManagerDto("Updated Name", "Updated Description", "Updated Location", false);

      // Act
      var profile = await _managerService.UpdateAsync(managerProfile!.Id, dto);

      // Assert
      var allProfiles = _context.ManagerProfiles.ToList();

      Assert.Single(allProfiles);

      Assert.Equal(managerProfile.Id, profile.Id);
      Assert.Equal("Updated Name", profile.Name);
      Assert.Equal("Updated Description", profile.Description);
      Assert.Equal("Updated Location", profile.Location);
      Assert.False(profile.Public);
    }

    [Fact]
    public async Task CanDelete()
    {
      // Arrange
      var user = f.AddFakeUser()!;
      var profile = f.AddFakeManagerProfile(user.Id)!;
      var gallery = f.AddFakeGallery(profile.MediaGalleryOwnerId)!;
      var item = f.AddFakeMediaItem(gallery.Id)!;
      var link = item.MediaLink!;

      var profileToKeep = f.AddFakeManagerProfile(user.Id)!;

      // Act
      await _managerService.DeleteAsync(profile.Id);

      // Assert
      var allProfiles = _context.ManagerProfiles.ToList();
      var allGalleryOwners = _context.MediaGalleryOwners.ToList();
      var allGalleries = _context.MediaGalleries.ToList();
      var allItems = _context.MediaItems.ToList();
      var allLinks = _context.MediaLinks.ToList();

      Assert.Single(allProfiles);
      Assert.Equal(allProfiles.First(), profileToKeep);

      Assert.Single(allGalleryOwners);
      Assert.Empty(allGalleries);
      Assert.Empty(allItems);
      Assert.Empty(allLinks);
    }
  }
}
