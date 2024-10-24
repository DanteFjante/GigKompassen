using GigKompassen.Models.Profiles;
using GigKompassen.Test.Helpers;


namespace GigKompassen.Test.Tests
{
  public class TestEntityRepositoryTests
  {
    public TestEntityRepositoryTests()
    {
    }

    private TestEntityRepository GetRepo()
    {
      return new TestEntityRepository();
    }

    [Fact]
    public async Task CanGetUser()
    {
      // Arrange
      var repo = GetRepo();
      var userId = Guid.NewGuid();
      var participantId1 = Guid.NewGuid();
      var participantId2 = Guid.NewGuid();
      var mediaLinkId1 = Guid.NewGuid();
      var mediaLinkId2 = Guid.NewGuid();
      var profileAccessId1 = Guid.NewGuid();
      var profileAccessId2 = Guid.NewGuid();

      // Act
      var user = repo.GetUser(
        id: userId,
        participants: new() { repo.NewChatParticipant(participantId1), repo.NewChatParticipant(participantId2) },
        mediaLinks: new() { repo.NewMediaLink(mediaLinkId1), repo.NewMediaLink(mediaLinkId2) },
        profileAccesses: new() { repo.NewProfileAccess(profileAccessId1), repo.NewProfileAccess(profileAccessId2) }
        );

      // Assert
      //User1
      Assert.NotNull(user);
      Assert.Equal(userId, user.Id);
      Assert.Contains(repo.ChatParticipant, p => p.Id == participantId1);
      Assert.Contains(repo.ChatParticipant, p => p.Id == participantId2);
      Assert.Contains(user.ChatParticipations!, p => p.Id == participantId1);
      Assert.Contains(user.ChatParticipations!, p => p.Id == participantId2);

      Assert.Contains(repo.MediaLinks, p => p.Id == mediaLinkId1);
      Assert.Contains(repo.MediaLinks, p => p.Id == mediaLinkId2);
      Assert.Contains(user.UploadedMedia!, p => p.Id == mediaLinkId1);
      Assert.Contains(user.UploadedMedia!, p => p.Id == mediaLinkId2);

      Assert.Contains(repo.ProfilesAccesses, p => p.Id == profileAccessId1);
      Assert.Contains(repo.ProfilesAccesses, p => p.Id == profileAccessId2);
      Assert.Contains(user.ProfilesAccesses!, p => p.Id == profileAccessId1);
      Assert.Contains(user.ProfilesAccesses!, p => p.Id == profileAccessId2);

      Assert.True(user.ChatParticipations!.Count == 2);
      Assert.True(user.UploadedMedia!.Count == 2);
      Assert.True(user.ProfilesAccesses!.Count == 2);

    }

    [Fact]
    public void CanGetProfileAccess()
    {
      // Arrange
      var repo = GetRepo();
      var profileAccessId = Guid.NewGuid();
      var userId = Guid.NewGuid();
      var profileId = Guid.NewGuid();

      // Act
      var profileAccess = repo.GetProfileAccess(
        id: profileAccessId,
        user: repo.NewUser(userId),
        profile: repo.NewArtistProfile(profileId)
      );

      // Assert
      Profile? profile = repo.GetProfile(profileId);
      Assert.NotNull(profileAccess);
      Assert.Equal(profile.Id, profileId);

      Assert.Equal(profileAccessId, profileAccess.Id);

    }

    [Fact]
    public async Task CanGetProfiles()
    {
      // Arrange
      var repo = GetRepo();
      var profile1Id = Guid.NewGuid();
      var profile2Id = Guid.NewGuid();
      var profile3Id = Guid.NewGuid();

      var profile4Id = Guid.NewGuid();
      var profile5Id = Guid.NewGuid();
      var profile6Id = Guid.NewGuid();
      var profile7Id = Guid.NewGuid();
      var profile8Id = Guid.NewGuid();

      // Act
      var profile1 = repo.NewManagerProfile(profile1Id);
      var profile2 = repo.NewArtistProfile(profile2Id);
      var profile3 = repo.NewSceneProfile(profile3Id);

      var profile4 = repo.GetProfile(profile4Id);
      var profile5 = repo.GetProfile(profile5Id, type: Enums.ProfileTypes.Artist);

      var profile6 = repo.GetSceneProfile(profile6Id);
      var profile7 = repo.GetArtistProfile(profile7Id);
      var profile8 = repo.GetManagerProfile(profile8Id);


      // Assert
      Assert.IsType<ManagerProfile>(profile1);
      Assert.IsType<ArtistProfile>(profile2);
      Assert.IsType<SceneProfile>(profile3);

      Assert.IsType<ManagerProfile>(profile4);
      Assert.IsType<ArtistProfile>(profile5);
      Assert.IsType<SceneProfile>(profile6);

      Assert.Contains(repo.Profiles.OfType<ManagerProfile>(), p => p.Id == profile1Id);
      Assert.Contains(repo.Profiles.OfType<ArtistProfile>(), p => p.Id == profile2Id);
      Assert.Contains(repo.Profiles.OfType<SceneProfile>(), p => p.Id == profile3Id);

      Assert.Contains(repo.Profiles.OfType<ManagerProfile>(), p => p.Id == profile4Id);
      Assert.Contains(repo.Profiles.OfType<ArtistProfile>(), p => p.Id == profile5Id);

      Assert.Contains(repo.Profiles.OfType<SceneProfile>(), p => p.Id == profile6Id);
      Assert.Contains(repo.Profiles.OfType<ArtistProfile>(), p => p.Id == profile7Id);
      Assert.Contains(repo.Profiles.OfType<ManagerProfile>(), p => p.Id == profile8Id);

    }

    [Fact]
    public async Task CanGetArtistProfile()
    {
      // Arrange
      var repo = GetRepo();
      var profileId = Guid.NewGuid();
      // Act
      var profile = repo.GetArtistProfile(profileId, repo.NewMediaGalleryOwner(), repo.NewUser(), new() { repo.NewArtistMember() }, new() { repo.NewGenre() }, new() { repo.NewProfileAccess() });
      // Assert
      Assert.NotNull(profile);
      Assert.Equal(profileId, profile.Id);
      Assert.NotNull(profile.GalleryOwner);
      Assert.True(profile.Members!.Count == 1);
      Assert.Contains(repo.Genres, p => p.Id == profile.Genres!.First().Id);
      Assert.True(repo.Genres.First().ArtistProfiles!.Count == 1);
      Assert.True(repo.Genres.First().ArtistProfiles!.First().Id == profile.Id);
    }

    [Fact]
    public async Task CanChangeProfileValue()
    {
      // Arrange
      var repo = GetRepo();
      var profileId = Guid.NewGuid();
      // Act
      var profile = repo.NewSceneProfile(profileId);
      profile.Name = "NewName";

      var newProfile = repo.GetSceneProfile(profileId);
      // Assert
      Assert.Equal(profileId, newProfile.Id);
      Assert.Equal("NewName", newProfile.Name);
    }

    [Fact]
    public async Task CountWorks()
    {
      // Arrange
      var repo = GetRepo();
      
      // Act
      var user1 = repo.NewUser();
      var user2 = repo.NewUser();

      // Assert
      Assert.Equal(2, repo.Users.Count());
      Assert.Equal("test0", user1.UserName);
      Assert.Equal("test1", user2.UserName);
    }
  }
}
