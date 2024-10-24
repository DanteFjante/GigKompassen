using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using Xunit;

namespace GigKompassen.Test.Tests
{

  public class ApplicationDbContextTests
  {
    // Add more tests to cover other entities and relationships
    private ApplicationDbContext GetInMemoryDbContext()
    {
      return DbContextHelper.GetInMemoryDbContext();
    }

    #region Accounts
    #region Adding

    [Fact]
    public async Task CanAddAndRetrieveApplicationUser()
    {
      TestEntityRepository repo = new TestEntityRepository();
      // Arrange
      var context = GetInMemoryDbContext();

      var mediaLink = new List<MediaLink>() { repo.NewMediaLink() };
      var profileAccess = new List<ProfileAccess>() { repo.NewProfileAccess() };
      var participant = new List<ChatParticipant>() { repo.NewChatParticipant() };
      var profile = new List<Profile>() { repo.GetProfile(profileAccesses: profileAccess) };

      var user = repo.GetUser(Guid.NewGuid(), profile, participant, mediaLink, profileAccess);
      var username = "testUser";
      user.UserName = username;

      // Act
      context.Users.Add(user);
      await context.SaveChangesAsync();

      // Assert
      var retrievedUser = await context.Users.FindAsync(user.Id);
      var retrievedParticipant = await context.ChatParticipants.FindAsync(participant[0].Id);
      var retrievedMediaLink = await context.MediaLinks.FindAsync(mediaLink[0].Id);
      var retrievedProfileAccess = await context.ProfileAccesses.FindAsync(profileAccess[0].Id);
      var retrievedProfile = await context.Profiles.FindAsync(profile[0].Id);

      Assert.NotNull(retrievedUser);
      Assert.NotNull(retrievedParticipant);
      Assert.NotNull(retrievedMediaLink);
      Assert.NotNull(retrievedProfileAccess);
      Assert.NotNull(retrievedProfile);
      Assert.Equal(retrievedProfile.Id, profile[0].Id);
      Assert.Equal(username, retrievedUser.UserName);
    }

    [Fact]
    public async Task CanAddAndRetrieveProfileAccess()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var profileAccess = repo.GetProfileAccess(user: user, profile: profile);

      // Act
      context.ProfileAccesses.Add(profileAccess);
      await context.SaveChangesAsync();

      // Assert
      var retrievedAccess = await context.ProfileAccesses.FindAsync(profileAccess.Id);
      var retrievedUser = await context.Users.FindAsync(user.Id);
      var retrievedProfile = await context.Profiles.FindAsync(profile.Id);
      Assert.NotNull(retrievedAccess);
      Assert.NotNull(retrievedProfile);
      Assert.NotNull(retrievedUser);
    }

    #endregion
    #region Updating
    #endregion
    #region Deleting
    [Fact]
    public async Task DeletingApplicationUser_CascadesToProfileAccess()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var profile = repo.NewArtistProfile();
      var user = repo.NewUser();
      var profileAccess = repo.GetProfileAccess(user: user, profile: profile);  

      context.Users.Add(user);
      await context.SaveChangesAsync();

      // Act
      context.Users.Remove(user);
      await context.SaveChangesAsync();

      // Assert
      bool hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      bool hasProfile = await context.Profiles.AnyAsync(p => p.Id == profile.Id);
      bool hasProfileAccess = await context.ProfileAccesses.AnyAsync(p => p.Id == profileAccess.Id);
      Assert.True(hasProfile);
      Assert.False(hasUser);
      Assert.False(hasProfileAccess);
    }

    [Fact]
    public async Task DeletingApplicationUser_CascadesToChatParticipants()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var participant = repo.NewChatParticipant();
      var user = repo.GetUser(participants: new() { participant });

      context.Users.Add(user);
      await context.SaveChangesAsync();

      //Assert
      bool hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      bool hasParticipant = await context.ChatParticipants.AnyAsync(p => p.Id == participant.Id);

      Assert.True(hasParticipant);
      Assert.True(hasUser);

      // Act
      context.Users.Remove(user);
      await context.SaveChangesAsync();

      // Assert
      hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      hasParticipant = await context.ChatParticipants.AnyAsync(p => p.Id == participant.Id);
      Assert.False(hasUser);
      Assert.False(hasParticipant);
    }

    [Fact]
    public async Task DeletingApplicationUser_CascadesToMediaLinks()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var profile = repo.NewArtistProfile();
      var user = repo.GetUser(ownedProfiles: new() { profile });

      context.Users.Add(user);
      await context.SaveChangesAsync();

      // Act
      context.Users.Remove(user);
      await context.SaveChangesAsync();

      // Assert
      bool hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      bool hasProfile = await context.Profiles.AnyAsync(p => p.Id == profile.Id);
      Assert.False(hasUser);
      Assert.False(hasProfile);
    }

    [Fact]
    public async Task DeletingApplicationUser_CascadesToOwnedProfiles()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var profile = repo.NewArtistProfile();
      var user = repo.GetUser(ownedProfiles: new() { profile });

      context.Users.Add(user);
      await context.SaveChangesAsync();

      // Act
      context.Users.Remove(user);
      await context.SaveChangesAsync();

      // Assert
      bool hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      bool hasProfile = await context.Profiles.AnyAsync(p => p.Id == profile.Id);
      Assert.False(hasUser);
      Assert.False(hasProfile);
    }

    [Fact]
    public async Task DeletingProfileAccess_DoesNotCascade()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();

      var user = repo.NewUser();
      var profile = repo.NewArtistProfile();
      var profileAccess = repo.GetProfileAccess(user: user, profile: profile);

      context.ProfileAccesses.Add(profileAccess);
      await context.SaveChangesAsync();

      // Act
      context.ProfileAccesses.Remove(profileAccess);
      await context.SaveChangesAsync();

      // Assert
      bool hasProfileAccess = await context.ProfileAccesses.AnyAsync(p => p.Id == profileAccess.Id);
      bool hasUser = await context.Users.AnyAsync(p => p.Id == user.Id);
      bool hasProfile = await context.Profiles.AnyAsync(p => p.Id == profile.Id);
      Assert.False(hasProfileAccess);
      Assert.True(hasUser);
      Assert.True(hasProfile);
    }

    #endregion
    #endregion

    #region Profiles
    #region Adding
    [Fact]
    public async Task AddAndRetrieveProfileSubtypes()
    {
    }
    #endregion

    #region Updating
    #endregion

    #region Deleting
    [Fact]
    public async Task DeletingProfile_CascadesToProfileAccess()
    {
    }
    #endregion
    #endregion

    #region Chats
    #region Adding
    [Fact]
    public async Task CanAddAndRetrieveChatWithParticipantsAndMessages()
    {
    }
    #endregion
    #region Updating
    #endregion
    #region Deleting
    [Fact]
    public async Task DeletingChat_CascadesToParticipantsAndMessages()
    {
    }

    [Fact]
    public async Task DeletingChatParticipant_CascadesToMessages()
    {
    }
    #endregion
    #endregion

    #region Media
    #region Adding
    #endregion
    #region Updating
    #endregion
    #region Deleting
    #endregion
    #endregion

  }

}
