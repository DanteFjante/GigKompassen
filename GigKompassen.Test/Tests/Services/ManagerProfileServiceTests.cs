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
  public class ManagerProfileServiceTests
  {
    [Fact]
    public async Task CanGetAll()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new ManagerProfileService(context);

      var user = repo.NewUser();
      var profile1 = repo.GetManagerProfile(owner: user);
      var profile2 = repo.GetManagerProfile(owner: user);
      var profile3 = repo.GetManagerProfile(owner: user);
      var profile4 = repo.GetManagerProfile(owner: user);

      context.ManagerProfiles.AddRange(profile1, profile2, profile3, profile4);
      context.SaveChanges();

      // Act
      var profiles = await service.GetAllAsync();

      // Assert
      Assert.Equal(4, profiles.Count);
      Assert.Contains(profile1, profiles);
      Assert.Contains(profile2, profiles);
      Assert.Contains(profile3, profiles);
      Assert.Contains(profile4, profiles);

    }

    [Fact]
    public async Task CanGetById()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new ManagerProfileService(context);

      var user = repo.NewUser();
      var profile1 = repo.GetManagerProfile(owner: user);
      var profile2 = repo.GetManagerProfile(owner: user);
      var profile3 = repo.GetManagerProfile(owner: user);
      var profile4 = repo.GetManagerProfile(owner: user);

      context.ManagerProfiles.AddRange(profile1, profile2, profile3, profile4);
      context.SaveChanges();

      // Act
      var retrievedProfile1 = await service.GetAsync(profile1.Id);
      var retrievedProfile2 = await service.GetAsync(profile2.Id);
      var retrievedProfile3 = await service.GetAsync(profile3.Id);
      var retrievedProfile4 = await service.GetAsync(profile4.Id);
      var nullProfile = await service.GetAsync(Guid.NewGuid());

      // Assert
      Assert.Equal(profile1, retrievedProfile1);
      Assert.Equal(profile2, retrievedProfile2);
      Assert.Equal(profile3, retrievedProfile3);
      Assert.Equal(profile4, retrievedProfile4);
      Assert.Null(nullProfile);
    }

    [Fact]
    public async Task CanCreate()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new ManagerProfileService(context);

      var user = repo.NewUser();
      var fakeUser = repo.NewUser();
      var profile1 = new ManagerProfileDto()
      {
        Description = "Description 1",
        Location = "Location 1",
        Name = "Profile 1"
      };

      var profile2 = new ManagerProfileDto()
      {
        Description = "Description 2",
        Location = "Location 2",
        Name = "Profile 2"
      };

      context.Users.Add(user);
      context.SaveChanges();

      // Act
      var retrievedProfile = await service.CreateAsync(user.Id, profile1);

      // Assert
      await Assert.ThrowsAsync<KeyNotFoundException>(() => service.CreateAsync(fakeUser.Id, profile1));
      Assert.NotNull(retrievedProfile);
      Assert.Equal(profile1.Name, retrievedProfile.Name);
      Assert.Equal(profile1.Description, retrievedProfile.Description);
      Assert.Equal(profile1.Location, retrievedProfile.Location);
      Assert.Equal(user, retrievedProfile.Owner);
    }

    [Fact]
    public async Task CanUpdate()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new ManagerProfileService(context);

      var user = repo.NewUser();
      var profile = repo.GetManagerProfile(owner: user);

      var profileDto = new ManagerProfileDto()
      {
        Description = "New Description",
        Location = "New Location",
        Name = "New Name"
      };

      context.ManagerProfiles.AddRange(profile);
      context.SaveChanges();

      // Act
      await service.UpdateAsync(profile.Id, profileDto);
      var retrievedProfile = context.ManagerProfiles.FirstOrDefault(p => p.Id == profile.Id);

      // Assert
      await Assert.ThrowsAsync<KeyNotFoundException>(() => service.UpdateAsync(Guid.NewGuid(), profileDto));
      Assert.NotNull(retrievedProfile);
      Assert.Equal(profileDto.Name, retrievedProfile.Name);
      Assert.Equal(profileDto.Description, retrievedProfile.Description);
      Assert.Equal(profileDto.Location, retrievedProfile.Location);

    }

    [Fact]
    public async Task CanDelete()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new ManagerProfileService(context);

      var user = repo.NewUser();
      var profile = repo.GetManagerProfile(owner: user);

      context.ManagerProfiles.AddRange(profile);
      context.SaveChanges();

      // Act
      var couldDelete = await service.DeleteAsync(profile.Id);
      var couldntDelete = await service.DeleteAsync(Guid.NewGuid());

      var profiles = context.ManagerProfiles.ToList();

      // Assert
      Assert.True(couldDelete);
      Assert.False(couldntDelete);
      Assert.Empty(profiles);
    }
  }
}
