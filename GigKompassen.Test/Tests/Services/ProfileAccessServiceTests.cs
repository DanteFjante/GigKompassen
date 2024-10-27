using GigKompassen.Enums;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Test.Tests.Services
{
  public class ProfileAccessServiceTests
  {

    public ProfileAccessServiceTests() { }

    [Fact]
    public async Task CanCheckIfAuthExists()
    {

      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();

      var user1 = repo.NewUser();
      var user2 = repo.NewUser();
      var openProfile = repo.NewArtistProfile();
      var closedProfile = repo.NewArtistProfile();
      closedProfile.Public = false;

      var access1 = repo.GetProfileAccess(Guid.NewGuid(), user1, openProfile);
      access1.AccessType = AccessType.Delete;
      
      var access2 = repo.GetProfileAccess(Guid.NewGuid(), user1, closedProfile);
      access2.AccessType = AccessType.Delete;
      
      var access3 = repo.GetProfileAccess(Guid.NewGuid(), user2, closedProfile);
      access3.AccessType = AccessType.View;

      // Act
      context.Users.Add(user1);
      context.Users.Add(user2);
      context.Profiles.Add(openProfile);
      context.Profiles.Add(closedProfile);
      context.ProfileAccesses.Add(access1);
      context.ProfileAccesses.Add(access2);
      context.ProfileAccesses.Add(access3);
      context.SaveChanges();


      bool result01 = await service.CanAccessProfile(openProfile.Id, user1.Id, AccessType.Delete);
      bool result02 = await service.CanAccessProfile(openProfile.Id, user1.Id, AccessType.View);
      bool result03 = await service.CanAccessProfile(openProfile.Id, user1.Id, AccessType.Edit);

      bool result04 = await service.CanAccessProfile(openProfile.Id, user2.Id, AccessType.Delete);
      bool result05 = await service.CanAccessProfile(openProfile.Id, user2.Id, AccessType.View);
      bool result06 = await service.CanAccessProfile(openProfile.Id, user2.Id, AccessType.Edit);

      bool result07 = await service.CanAccessProfile(closedProfile.Id, user1.Id, AccessType.Delete);
      bool result08 = await service.CanAccessProfile(closedProfile.Id, user1.Id, AccessType.View);
      bool result09 = await service.CanAccessProfile(closedProfile.Id, user1.Id, AccessType.Edit);

      bool result10 = await service.CanAccessProfile(closedProfile.Id, user2.Id, AccessType.Delete);
      bool result11 = await service.CanAccessProfile(closedProfile.Id, user2.Id, AccessType.View);
      bool result12 = await service.CanAccessProfile(closedProfile.Id, user2.Id, AccessType.Edit);

      // Assert
      Assert.True(result01);
      Assert.True(result02);
      Assert.False(result03);

      Assert.False(result04);
      Assert.True(result05);
      Assert.False(result06);

      Assert.True(result07);
      Assert.False(result08);
      Assert.False(result09);

      Assert.False(result10);
      Assert.True(result11);
      Assert.False(result12);


    }

    [Fact]
    public async Task CanAddAuth()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();

      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.SaveChanges();
      var result = await service.AddProfileAuthorization(profile.Id, user.Id, Enums.AccessType.Delete);

      // Assert
      Assert.True(result);

    }

    [Fact]
    public async Task CanRemoveAuth()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var access = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.ProfileAccesses.Add(access);
      context.SaveChanges();
      var result = await service.RemoveAuthorization(profile, user);
      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task CanRemoveAuthByType()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var access = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      access.AccessType = AccessType.Delete;
      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.ProfileAccesses.Add(access);
      context.SaveChanges();
      var result = await service.RemoveAuthorization(profile, user, AccessType.Delete);
      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task CanRemoveAuthById()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var access = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.ProfileAccesses.Add(access);
      context.SaveChanges();
      var result = await service.RemoveAuthorization(access.Id);
      // Assert
      Assert.True(result);
    }

    [Fact]
    public async Task CanClearAuthFromProfile()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var access1 = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      access1.AccessType = AccessType.Delete;
      var access2 = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      access2.AccessType = AccessType.View;
      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.ProfileAccesses.Add(access1);
      context.ProfileAccesses.Add(access2);
      context.SaveChanges();
      var result = await service.ClearAuthorizations(profile);
      // Assert
      Assert.True(result);
      Assert.Empty(context.ProfileAccesses);
    }

    [Fact]
    public async Task CanClearAuthFromUser()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var access1 = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      access1.AccessType = AccessType.Delete;
      var access2 = repo.GetProfileAccess(Guid.NewGuid(), user, profile);
      access2.AccessType = AccessType.View;
      // Act
      context.Users.Add(user);
      context.Profiles.Add(profile);
      context.ProfileAccesses.Add(access1);
      context.ProfileAccesses.Add(access2);
      context.SaveChanges();
      var result = await service.ClearAuthorizations(user);
      // Assert
      Assert.True(result);
      Assert.Empty(context.ProfileAccesses);
    } 

    [Fact]
    public async Task CanSetOwner()
    {
      // Arrange
      var context = DbContextHelper.GetInMemoryDbContext();
      var service = new ProfileAccessService(context);
      var repo = new TestEntityRepository();
      var olduser = repo.NewUser();
      var newuser = repo.NewUser();
      var profile = repo.NewArtistProfile();

      // Act
      context.Users.Add(olduser);
      context.Users.Add(newuser);
      context.Profiles.Add(profile);
      context.SaveChanges();

      var result = await service.SetProfileOwner(profile, newuser);
      var retrievedProfile = await context.Profiles.FirstOrDefaultAsync(p => p.Id == profile.Id);
      var retrievedOldUser = await context.Users.Include(p => p.OwnedProfiles).FirstOrDefaultAsync(u => u.Id == olduser.Id);
      var retrievedUser = retrievedProfile?.Owner;

      // Assert
      Assert.NotNull(retrievedProfile);
      Assert.NotNull(retrievedUser);
      Assert.NotNull(retrievedOldUser);
      Assert.NotNull(retrievedOldUser.OwnedProfiles);
      Assert.True(result);
      Assert.Equal(newuser.Id, retrievedUser.Id);
      Assert.Empty(retrievedOldUser.OwnedProfiles);


    }
  }
}
