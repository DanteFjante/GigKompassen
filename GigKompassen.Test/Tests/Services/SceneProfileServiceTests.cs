using AutoMapper;

using GigKompassen.Dto.Profiles;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GigKompassen.Test.Tests.Services
{
  public class SceneProfileServiceTests
  {
    [Fact]
    public async Task CanGetAll()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var genreService = new GenreService(context);
      var mediaService = new MediaService(context);
      var service = new SceneProfileService(context, genreService, mediaService);

      var user = repo.NewUser();
      var profile1 = repo.GetSceneProfile(owner: user);
      var profile2 = repo.GetSceneProfile(owner: user);
      var profile3 = repo.GetSceneProfile(owner: user);

      context.SceneProfiles.AddRange(profile1, profile2, profile3);
      context.SaveChanges();

      // Act
      var profiles = await service.GetAllAsync();

      // Assert
      Assert.Equal(3, profiles.Count);
      Assert.Contains(profile1, profiles);
      Assert.Contains(profile2, profiles);
      Assert.Contains(profile3, profiles);

    }

    [Fact]
    public async Task CanGetById()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var genreService = new GenreService(context);
      var mediaService = new MediaService(context);
      var service = new SceneProfileService(context, genreService, mediaService);

      var user = repo.NewUser();
      var profile1 = repo.GetSceneProfile(owner: user);
      var profile2 = repo.GetSceneProfile(owner: user);
      var profile3 = repo.GetSceneProfile(owner: user);

      context.SceneProfiles.AddRange(profile1, profile2, profile3);
      context.SaveChanges();

      // Act
      var retrievedProfile1 = await service.GetAsync(profile1.Id);
      var retrievedProfile2 = await service.GetAsync(profile2.Id);
      var retrievedProfile3 = await service.GetAsync(profile3.Id);
      var retrievedProfile4 = await service.GetAsync(Guid.NewGuid());

      // Assert
      Assert.Equal(profile1, retrievedProfile1);
      Assert.Equal(profile2, retrievedProfile2);
      Assert.Equal(profile3, retrievedProfile3);
      Assert.Null(retrievedProfile4);
    }

    [Fact]
    public async Task CanCreate()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var genreService = new GenreService(context);
      var mediaService = new MediaService(context);
      var service = new SceneProfileService(context, genreService, mediaService);

      var user = repo.NewUser();
      var profile = new SceneProfileDto
      {
        Id = Guid.NewGuid(),
        Name = "Test Venue",
        Description = "Test Description",
        Address = "Test Address",
        VenueType = "Test Venue Type",
        ContactInfo = "Test Contact Info",
        Capacity = 100,
        Amenities = "Test Amenities",
        Bio = "Test Bio",
        OpeningHours = "Test Opening Hours",
        Genres = new List<GenreDto>
        {
          new GenreDto("Test Genre 1"),
          new GenreDto("Test Genre 2"),
          new GenreDto("Test Genre 3")
        }
      };

      context.Users.Add(user);
      context.SaveChanges();

      // Act
      var createdProfile = await service.CreateAsync(user.Id, profile);

      // Assert
      Assert.NotNull(createdProfile);
      await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateAsync(Guid.NewGuid(), profile));
      Assert.Equal(profile.Name, createdProfile.Name);
      Assert.Equal(profile.Description, createdProfile.Description);
      Assert.Equal(profile.Address, createdProfile.Address);
      Assert.Equal(profile.VenueType, createdProfile.VenueType);
      Assert.Equal(profile.ContactInfo, createdProfile.ContactInfo);
      Assert.Equal(profile.Capacity, createdProfile.Capacity);
      Assert.Equal(profile.Amenities, createdProfile.Amenities);
      Assert.Equal(profile.Bio, createdProfile.Bio);
      Assert.Equal(profile.OpeningHours, createdProfile.OpeningHours);
      Assert.Equal(profile.Genres.Count, createdProfile.Genres.Count);
      Assert.Equal(profile.Genres[0].Name, createdProfile.Genres[0].Name);
      Assert.Equal(profile.Genres[1].Name, createdProfile.Genres[1].Name);
      Assert.Equal(profile.Genres[2].Name, createdProfile.Genres[2].Name);
    }

    [Fact]
    public async Task CanUpdate()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var genreService = new GenreService(context);
      var mediaService = new MediaService(context);
      var service = new SceneProfileService(context, genreService, mediaService);

      var user = repo.NewUser();
      var profile = repo.GetSceneProfile(owner: user);

      var dto = new SceneProfileDto
      {
        Name = "Updated Name",
        Description = "Updated Description",
        Address = "Updated Address",
        VenueType = "Updated Venue Type",
        ContactInfo = "Updated Contact Info",
        Capacity = 200,
        Amenities = "Updated Amenities",
        Bio = "Updated Bio",
        OpeningHours = "Updated Opening Hours",
        Genres = new List<GenreDto>
        {
          new GenreDto("Updated Genre 1"),
          new GenreDto("Updated Genre 2"),
          new GenreDto("Updated Genre 3")
        }
      };

      context.SceneProfiles.AddRange(profile);
      context.Users.Add(user);
      context.SaveChanges();

      // Act
      await service.UpdateAsync(profile.Id, dto);
      var updatedProfile = context.SceneProfiles.FirstOrDefault(p => p.Id == profile.Id);

      // Assert
      Assert.NotNull(updatedProfile);
      Assert.NotNull(updatedProfile.Genres);
      Assert.Equal(3, updatedProfile.Genres.Count);
      Assert.Contains(updatedProfile.Genres, g => g.Name == "Updated Genre 1");
      Assert.Contains(updatedProfile.Genres, g => g.Name == "Updated Genre 2");
      Assert.Contains(updatedProfile.Genres, g => g.Name == "Updated Genre 3");
      Assert.Equal(dto.Name, updatedProfile.Name);
      Assert.Equal(dto.Description, updatedProfile.Description);
      Assert.Equal(dto.Address, updatedProfile.Address);
      Assert.Equal(dto.VenueType, updatedProfile.VenueType);
      Assert.Equal(dto.ContactInfo, updatedProfile.ContactInfo);
      Assert.Equal(dto.Capacity, updatedProfile.Capacity);
      Assert.Equal(dto.Amenities, updatedProfile.Amenities);
      Assert.Equal(dto.Bio, updatedProfile.Bio);
      Assert.Equal(dto.OpeningHours, updatedProfile.OpeningHours);

    }

    [Fact]
    public async Task CanDelete()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var genreService = new GenreService(context);
      var mediaService = new MediaService(context);
      var service = new SceneProfileService(context, genreService, mediaService);

      var user = repo.NewUser();
      var profile = repo.GetSceneProfile(owner: user);

      context.SceneProfiles.AddRange(profile);
      context.SaveChanges();

      // Act
      var didDelete = await service.DeleteAsync(profile.Id);
      var didNotDelete = await service.DeleteAsync(Guid.NewGuid());
      var profiles = context.SceneProfiles.ToList();

      // Assert
      Assert.Empty(profiles);
      Assert.True(didDelete);
      Assert.False(didNotDelete);
    }
  }
}
