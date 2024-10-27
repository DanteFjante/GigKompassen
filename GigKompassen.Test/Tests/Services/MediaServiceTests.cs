using GigKompassen.Dto.Media;
using GigKompassen.Enums;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using static GigKompassen.Test.Helpers.DbContextHelper;

namespace GigKompassen.Test.Tests.Services
{
  public class MediaServiceTests
  {

    [Fact]
    public async Task CanGetGallery()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var chat = repo.NewChat();
      var artist = repo.NewArtistProfile();
      var scene = repo.NewSceneProfile();

      var gallery1 = repo.NewMediaGallery();
      var gallery2 = repo.NewMediaGallery();
      var gallery3 = repo.NewMediaGallery();

      var owner1 = repo.GetMediaGalleryOwner(chat: chat, gallery: gallery1);
      var owner2 = repo.GetMediaGalleryOwner(artistProfile: artist, gallery: gallery2);
      var owner3 = repo.GetMediaGalleryOwner(sceneProfile: scene, gallery: gallery3);
      
      context.MediaGalleryOwners.AddRange(owner1, owner2, owner3);
      context.SaveChanges();

      // Act
      var result1 = await service.GetGalleryAsync(gallery1.Id);
      var result2 = await service.GetGalleryAsync(gallery2.Id);
      var result3 = await service.GetGalleryAsync(gallery3.Id);
      var nullResult = await service.GetGalleryAsync(Guid.NewGuid());

      // Assert
      Assert.NotNull(result1);
      Assert.Equal(gallery1.Id, result1.Id);
      Assert.NotNull(result1.Owner);
      Assert.NotNull(result1.Owner.Chat);
      Assert.Null(result1.Owner.ArtistProfile);
      Assert.Null(result1.Owner.SceneProfile);
      Assert.Equal(chat.Id, result1.Owner.Chat.Id);

      Assert.NotNull(result2);
      Assert.Equal(gallery2.Id, result2.Id);
      Assert.NotNull(result2.Owner);
      Assert.NotNull(result2.Owner.ArtistProfile);
      Assert.Null(result2.Owner.Chat);
      Assert.Null(result2.Owner.SceneProfile);
      Assert.Equal(artist.Id, result2.Owner.ArtistProfile.Id);

      Assert.NotNull(result3);
      Assert.Equal(gallery3.Id, result3.Id);
      Assert.NotNull(result3.Owner);
      Assert.NotNull(result3.Owner.SceneProfile);
      Assert.Null(result3.Owner.Chat);
      Assert.Null(result3.Owner.ArtistProfile);
      Assert.Equal(scene.Id, result3.Owner.SceneProfile.Id);

      Assert.Null(nullResult);
    }

    [Fact]
    public async Task CanGetMediaItem()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var chat = repo.NewChat();
      var artist = repo.NewArtistProfile();
      var scene = repo.NewSceneProfile();

      var gallery1 = repo.NewMediaGallery();
      var gallery2 = repo.NewMediaGallery();
      var gallery3 = repo.NewMediaGallery();

      var link1 = repo.NewMediaLink();
      var link2 = repo.NewMediaLink();
      var link3 = repo.NewMediaLink();
      var link4 = repo.NewMediaLink();
      var link5 = repo.NewMediaLink();
      var link6 = repo.NewMediaLink();

      var item1 = repo.GetMediaItem(link: link1, gallery: gallery1);
      var item2 = repo.GetMediaItem(link: link2, gallery: gallery1);
      var item3 = repo.GetMediaItem(link: link3, gallery: gallery2);
      var item4 = repo.GetMediaItem(link: link4, gallery: gallery2);
      var item5 = repo.GetMediaItem(link: link5, gallery: gallery3);
      var item6 = repo.GetMediaItem(link: link6, gallery: gallery3);

      var owner1 = repo.GetMediaGalleryOwner(chat: chat, gallery: gallery1);
      var owner2 = repo.GetMediaGalleryOwner(artistProfile: artist, gallery: gallery2);
      var owner3 = repo.GetMediaGalleryOwner(sceneProfile: scene, gallery: gallery3);

      context.MediaGalleryOwners.AddRange(owner1, owner2, owner3);
      context.SaveChanges();

      // Act
      var result1 = await service.GetMediaItemAsync(item1.Id);
      var result2 = await service.GetMediaItemAsync(item2.Id);
      var result3 = await service.GetMediaItemAsync(item3.Id);
      var result4 = await service.GetMediaItemAsync(item4.Id);
      var result5 = await service.GetMediaItemAsync(item5.Id);
      var result6 = await service.GetMediaItemAsync(item6.Id);
      var nullResult = await service.GetMediaItemAsync(Guid.NewGuid());

      // Assert
      Assert.NotNull(result1);
      Assert.Equal(item1.Id, result1.Id);
      Assert.NotNull(result1.Link);
      Assert.Equal(link1.Id, result1.Link.Id);
      Assert.NotNull(result1.Gallery);
      Assert.Equal(gallery1.Id, result1.Gallery.Id);
      Assert.NotNull(result1.Gallery.Owner);
      Assert.NotNull(result1.Gallery.Owner.Chat);
      Assert.Null(result1.Gallery.Owner.ArtistProfile);
      Assert.Null(result1.Gallery.Owner.SceneProfile);
      Assert.Equal(chat.Id, result1.Gallery.Owner.Chat.Id);

      Assert.NotNull(result2);
      Assert.Equal(item2.Id, result2.Id);
      Assert.NotNull(result2.Link);
      Assert.Equal(link2.Id, result2.Link.Id);
      Assert.NotNull(result2.Gallery);
      Assert.Equal(gallery1.Id, result2.Gallery.Id);
      Assert.NotNull(result2.Gallery.Owner);
      Assert.NotNull(result2.Gallery.Owner.Chat);
      Assert.Null(result2.Gallery.Owner.ArtistProfile);
      Assert.Null(result2.Gallery.Owner.SceneProfile);
      Assert.Equal(chat.Id, result2.Gallery.Owner.Chat.Id);

      Assert.NotNull(result3);
      Assert.Equal(item3.Id, result3.Id);
      Assert.NotNull(result3.Link);
      Assert.Equal(link3.Id, result3.Link.Id);
      Assert.NotNull(result3.Gallery);
      Assert.Equal(gallery2.Id, result3.Gallery.Id);
      Assert.NotNull(result3.Gallery.Owner);
      Assert.NotNull(result3.Gallery.Owner.ArtistProfile);
      Assert.Null(result3.Gallery.Owner.Chat);
      Assert.Null(result3.Gallery.Owner.SceneProfile);
      Assert.Equal(artist.Id, result3.Gallery.Owner.ArtistProfile.Id);

      Assert.NotNull(result4);
      Assert.Equal(item4.Id, result4.Id);
      Assert.NotNull(result4.Link);
      Assert.Equal(link4.Id, result4.Link.Id);
      Assert.NotNull(result4.Gallery);
      Assert.Equal(gallery2.Id, result4.Gallery.Id);
      Assert.NotNull(result4.Gallery.Owner);
      Assert.NotNull(result4.Gallery.Owner.ArtistProfile);
      Assert.Null(result4.Gallery.Owner.Chat);
      Assert.Null(result4.Gallery.Owner.SceneProfile);
      Assert.Equal(artist.Id, result4.Gallery.Owner.ArtistProfile.Id);

      Assert.NotNull(result5);
      Assert.Equal(item5.Id, result5.Id);
      Assert.NotNull(result5.Link);
      Assert.Equal(link5.Id, result5.Link.Id);
      Assert.NotNull(result5.Gallery);
      Assert.Equal(gallery3.Id, result5.Gallery.Id);
      Assert.NotNull(result5.Gallery.Owner);
      Assert.NotNull(result5.Gallery.Owner.SceneProfile);
      Assert.Null(result5.Gallery.Owner.Chat);
      Assert.Null(result5.Gallery.Owner.ArtistProfile);
      Assert.Equal(scene.Id, result5.Gallery.Owner.SceneProfile.Id);
      
      Assert.NotNull(result6);
      Assert.Equal(item6.Id, result6.Id);
      Assert.NotNull(result6.Link);
      Assert.Equal(link6.Id, result6.Link.Id);
      Assert.NotNull(result6.Gallery);
      Assert.Equal(gallery3.Id, result6.Gallery.Id);
      Assert.NotNull(result6.Gallery.Owner);
      Assert.NotNull(result6.Gallery.Owner.SceneProfile);
      Assert.Null(result6.Gallery.Owner.Chat);
      Assert.Null(result6.Gallery.Owner.ArtistProfile);
      Assert.Equal(scene.Id, result6.Gallery.Owner.SceneProfile.Id);

      Assert.Null(nullResult);

    }

    [Fact]
    public async Task CanGetMediaLink()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user1 = repo.NewUser();
      var user2 = repo.NewUser();

      var link1 = repo.GetMediaLink(uploader: user1);
      var link2 = repo.GetMediaLink(uploader: user1);
      var link3 = repo.GetMediaLink(uploader: user2);
      var link4 = repo.GetMediaLink(uploader: user2);

      context.MediaLinks.AddRange(link1, link2, link3, link4);
      context.SaveChanges();

      // Act
      var result1 = await service.GetMediaLinkAsync(link1.Id);
      var result2 = await service.GetMediaLinkAsync(link2.Id);
      var result3 = await service.GetMediaLinkAsync(link3.Id);
      var result4 = await service.GetMediaLinkAsync(link4.Id);
      var nullResult = await service.GetMediaLinkAsync(Guid.NewGuid());

      // Assert
      Assert.NotNull(result1);
      Assert.Equal(link1.Id, result1.Id);
      Assert.NotNull(result1.Uploader);
      Assert.Equal(user1.Id, result1.Uploader.Id);
      Assert.NotNull(result2);
      Assert.Equal(link2.Id, result2.Id);
      Assert.NotNull(result2.Uploader);
      Assert.Equal(user1.Id, result2.Uploader.Id);
      Assert.NotNull(result3);
      Assert.Equal(link3.Id, result3.Id);
      Assert.NotNull(result3.Uploader);
      Assert.Equal(user2.Id, result3.Uploader.Id);
      Assert.NotNull(result4);
      Assert.Equal(link4.Id, result4.Id);
      Assert.NotNull(result4.Uploader);
      Assert.Equal(user2.Id, result4.Uploader.Id);
      Assert.Null(nullResult);
    }

    [Fact]
    public async Task CanCreateGallery()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var chat = repo.NewChat();
      var artist = repo.NewArtistProfile();
      var scene = repo.NewSceneProfile();

      context.Chats.Add(chat);
      context.ArtistProfiles.Add(artist);
      context.SceneProfiles.Add(scene);
      context.SaveChanges();

      // Act
      var result1 = await service.CreateGalleryOwnerAsync(chat);
      var result2 = await service.CreateGalleryOwnerAsync(artist);
      var result3 = await service.CreateGalleryOwnerAsync(scene);

      // Assert
      Assert.NotNull(result1);
      Assert.NotNull(result1.Gallery);
      Assert.NotNull(result1.Chat);
      Assert.Null(result1.ArtistProfile);
      Assert.Null(result1.SceneProfile);
      Assert.Equal(chat.Id, result1.Chat.Id);

      Assert.NotNull(result2);
      Assert.NotNull(result2.Gallery);
      Assert.NotNull(result2.ArtistProfile);
      Assert.Null(result2.Chat);
      Assert.Null(result2.SceneProfile);
      Assert.Equal(artist.Id, result2.ArtistProfile.Id);

      Assert.NotNull(result3);
      Assert.NotNull(result3.Gallery);
      Assert.NotNull(result3.SceneProfile);
      Assert.Null(result3.Chat);
      Assert.Null(result3.ArtistProfile);
      Assert.Equal(scene.Id, result3.SceneProfile.Id);
    }

    [Fact]
    public async Task CanCreateMediaItem()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();

      var link1 = repo.GetMediaLink(uploader: user);
      var link2 = repo.GetMediaLink(uploader: user);
      var link3 = repo.GetMediaLink(uploader: user);

      var chat = repo.NewChat();
      var artist = repo.NewArtistProfile();
      var scene = repo.NewSceneProfile();

      var gallery1 = repo.NewMediaGallery();
      var gallery2 = repo.NewMediaGallery();
      var gallery3 = repo.NewMediaGallery();

      var galleryOwner1 = repo.GetMediaGalleryOwner(chat: chat, gallery: gallery1);
      var galleryOwner2 = repo.GetMediaGalleryOwner(artistProfile: artist, gallery: gallery2);
      var galleryOwner3 = repo.GetMediaGalleryOwner(sceneProfile: scene, gallery: gallery3);

      context.MediaLinks.AddRange(link1, link2, link3);
      context.MediaGalleryOwners.AddRange(galleryOwner1, galleryOwner2, galleryOwner3);
      context.SaveChanges();

      var mediaItem1 = new MediaItemDto
      {
        Title = "Test Item 1",
        Description = "Test Description 1"
      };

      var mediaItem2 = new MediaItemDto
      {
        Title = "Test Item 2",
        Description = "Test Description 2"
      };

      var mediaItem3 = new MediaItemDto
      {
        Title = "Test Item 3",
        Description = "Test Description 3"
      };

      // Act
      var result1 = await service.CreateMediaItemAsync(gallery1.Id, link1.Id, mediaItem1);
      var result2 = await service.CreateMediaItemAsync(gallery2.Id, link2.Id, mediaItem2);
      var result3 = await service.CreateMediaItemAsync(gallery3.Id, link3.Id, mediaItem3);

      // Assert
      Assert.NotNull(result1);
      Assert.NotNull(result1.Gallery);
      Assert.Equal(gallery1.Id, result1.Gallery.Id);
      Assert.NotNull(result1.Link);
      Assert.Equal(link1.Id, result1.Link.Id);
      Assert.Equal(mediaItem1.Title, result1.Title);
      Assert.Equal(mediaItem1.Description, result1.Description);

      Assert.NotNull(result2);
      Assert.NotNull(result2.Gallery);
      Assert.Equal(gallery2.Id, result2.Gallery.Id);
      Assert.NotNull(result2.Link);
      Assert.Equal(link2.Id, result2.Link.Id);
      Assert.Equal(mediaItem2.Title, result2.Title);
      Assert.Equal(mediaItem2.Description, result2.Description);
      
      Assert.NotNull(result3);
      Assert.NotNull(result3.Gallery);
      Assert.Equal(gallery3.Id, result3.Gallery.Id);
      Assert.NotNull(result3.Link);
      Assert.Equal(link3.Id, result3.Link.Id);
      Assert.Equal(mediaItem3.Title, result3.Title);
      Assert.Equal(mediaItem3.Description, result3.Description);

    }

    [Fact]
    public async Task CanCreateMediaLink()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user1 = repo.NewUser();
      var user2 = repo.NewUser();

      MediaLinkDto mediaLink1 = new MediaLinkDto
      {
        MediaType = MediaType.Image,
        Path = "Test/Path/1"
      };

      MediaLinkDto mediaLink2 = new MediaLinkDto
      {
        MediaType = MediaType.Audio,
        Path = "Test/Path/2"
      };

      MediaLinkDto mediaLink3 = new MediaLinkDto
      {
        MediaType = MediaType.Video,
        Path = "Test/Path/3"
      };

      context.Users.AddRange(user1, user2);
      context.SaveChanges();

      // Act
      var result1 = await service.CreateMediaLinkAsync(user1.Id, mediaLink1);
      var result2 = await service.CreateMediaLinkAsync(user1.Id, mediaLink2);
      var result3 = await service.CreateMediaLinkAsync(user2.Id, mediaLink3);

      // Assert
      Assert.NotNull(result1);
      Assert.NotNull(result1.Uploader);
      Assert.Equal(user1.Id, result1.Uploader.Id);
      Assert.Equal(mediaLink1.MediaType, result1.MediaType);
      Assert.Equal(mediaLink1.Path, result1.Path);

      Assert.NotNull(result2);
      Assert.NotNull(result2.Uploader);
      Assert.Equal(user1.Id, result2.Uploader.Id);
      Assert.Equal(mediaLink2.MediaType, result2.MediaType);
      Assert.Equal(mediaLink2.Path, result2.Path);
      
      Assert.NotNull(result3);
      Assert.NotNull(result3.Uploader);
      Assert.Equal(user2.Id, result3.Uploader.Id);
      Assert.Equal(mediaLink3.MediaType, result3.MediaType);
      Assert.Equal(mediaLink3.Path, result3.Path);

    }
    
    [Fact]
    public async Task CanUpdateMediaItem()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();
      var link = repo.GetMediaLink(uploader: user);

      var gallery = repo.NewMediaGallery();
      var item = repo.GetMediaItem(link: link, gallery: gallery);

      context.MediaItems.Add(item);
      context.SaveChanges();

      var mediaItemDto = new MediaItemDto
      {
        Title = "Updated Title",
        Description = "Updated Description"
      };

      // Act
      var result = await service.UpdateMediaItemAsync(item.Id, mediaItemDto);

      // Assert
      Assert.NotNull(result);
      Assert.Equal(item.Id, result.Id);
      Assert.Equal(mediaItemDto.Title, result.Title);
      Assert.Equal(mediaItemDto.Description, result.Description);
    }

    [Fact]
    public async Task CanDeleteGalleryOwner()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();

      var chat = repo.NewChat();
      var artist = repo.NewArtistProfile();
      var scene = repo.NewSceneProfile();

      var gallery1 = repo.NewMediaGallery();
      var gallery2 = repo.NewMediaGallery();
      var gallery3 = repo.NewMediaGallery();

      var link1 = repo.GetMediaLink(uploader: user);
      var link2 = repo.GetMediaLink(uploader: user);
      var link3 = repo.GetMediaLink(uploader: user);

      var item1 = repo.GetMediaItem(link: link1, gallery: gallery1);
      var item2 = repo.GetMediaItem(link: link2, gallery: gallery2);
      var item3 = repo.GetMediaItem(link: link3, gallery: gallery3);

      var owner1 = repo.GetMediaGalleryOwner(chat: chat, gallery: gallery1);
      var owner2 = repo.GetMediaGalleryOwner(artistProfile: artist, gallery: gallery2);
      var owner3 = repo.GetMediaGalleryOwner(sceneProfile: scene, gallery: gallery3);

      context.MediaGalleryOwners.AddRange(owner1, owner2, owner3);
      context.SaveChanges();

      // Act
      var result1 = await service.DeleteGalleryOwnerAsync(owner1.Id);
      var result2 = await service.DeleteGalleryOwnerAsync(owner2.Id);
      var result3 = await service.DeleteGalleryOwnerAsync(owner3.Id);
      var nullResult = await service.DeleteGalleryOwnerAsync(Guid.NewGuid());

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.True(result3);
      Assert.False(nullResult);

      var galleries = context.MediaGalleries.ToList();
      Assert.Empty(galleries);

      var owners = context.MediaGalleryOwners.ToList();
      Assert.Empty(owners);

      var retrievedChat = context.Chats.FirstOrDefault(c => c.Id == chat.Id);
      Assert.NotNull(retrievedChat);
      Assert.Null(retrievedChat.GalleryOwner);

      var retrievedArtist = context.ArtistProfiles.FirstOrDefault(a => a.Id == artist.Id);
      Assert.NotNull(retrievedArtist);
      Assert.Null(retrievedArtist.GalleryOwner);

      var retrievedScene = context.SceneProfiles.FirstOrDefault(s => s.Id == scene.Id);
      Assert.NotNull(retrievedScene);
      Assert.Null(retrievedScene.GalleryOwner);

      var retrievedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
      Assert.NotNull(retrievedUser);
      Assert.NotNull(retrievedUser.UploadedMedia);
      Assert.Empty(retrievedUser.UploadedMedia);

      var galleryOwners = context.MediaGalleryOwners.ToList();
      Assert.Empty(galleryOwners);

      var mediaItems = context.MediaItems.ToList();
      Assert.Empty(mediaItems);

      var mediaLinks = context.MediaLinks.ToList();
      Assert.Empty(mediaLinks);


    }

    [Fact]
    async Task CanDeleteGallery()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();

      var gallery1 = repo.NewMediaGallery();
      var gallery2 = repo.NewMediaGallery();
      var gallery3 = repo.NewMediaGallery();

      var link1 = repo.GetMediaLink(uploader: user);
      var link2 = repo.GetMediaLink(uploader: user);
      var link3 = repo.GetMediaLink(uploader: user);

      var item1 = repo.GetMediaItem(link: link1, gallery: gallery1);
      var item2 = repo.GetMediaItem(link: link2, gallery: gallery2);
      var item3 = repo.GetMediaItem(link: link3, gallery: gallery3);

      context.MediaGalleries.AddRange(gallery1, gallery2, gallery3);
      context.SaveChanges();

      // Act
      var result1 = await service.DeleteGalleryAsync(gallery1.Id);
      var result2 = await service.DeleteGalleryAsync(gallery2.Id);
      var result3 = await service.DeleteGalleryAsync(gallery3.Id);
      var nullResult = await service.DeleteGalleryAsync(Guid.NewGuid());

      // Assert
      Assert.True(result1);
      Assert.True(result2);
      Assert.True(result3);

      var galleries = context.MediaGalleries.ToList();
      Assert.Empty(galleries);

      var mediaItems = context.MediaItems.ToList();
      Assert.Empty(mediaItems);

      var mediaLinks = context.MediaLinks.ToList();
      Assert.Empty(mediaLinks);

      var retrievedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
      Assert.NotNull(retrievedUser);

      Assert.False(nullResult);

    }

    [Fact]
    public async Task CanDeleteMediaItem()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();

      var link = repo.GetMediaLink(uploader: user);
      var link2 = repo.GetMediaLink(uploader: user);

      var gallery = repo.NewMediaGallery();
      var item = repo.GetMediaItem(link: link, gallery: gallery);
      var item2 = repo.GetMediaItem(link: link2, gallery: gallery);

      context.MediaItems.AddRange(item, item2);
      context.SaveChanges();

      // Act
      var result = await service.DeleteMediaItemAsync(item.Id);
      var nullResult = await service.DeleteMediaItemAsync(Guid.NewGuid());

      // Assert
      Assert.True(result);

      var mediaItems = context.MediaItems.ToList();
      Assert.Single(mediaItems);
      Assert.Equal(item2.Id, mediaItems.First().Id);

      var mediaLinks = context.MediaLinks.ToList();
      Assert.Single(mediaLinks);
      Assert.Equal(link2.Id, mediaLinks.First().Id);

      var retrievedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
      Assert.NotNull(retrievedUser);
      Assert.Single(mediaItems);
      Assert.Equal(link2.Id, retrievedUser.UploadedMedia.First().Id);

      var retrievedGallery = context.MediaGalleries.FirstOrDefault(g => g.Id == gallery.Id);
      Assert.NotNull(retrievedGallery);
      Assert.Single(retrievedGallery.Items);
      Assert.Equal(item2.Id, retrievedGallery.Items.First().Id);

      Assert.False(nullResult);

    }

    [Fact]
    public async Task CanDeleteMediaLink()
    {
      // Arrange
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var service = new MediaService(context);

      var user = repo.NewUser();

      var link1 = repo.GetMediaLink(uploader: user);
      var link2 = repo.GetMediaLink(uploader: user);

      context.MediaLinks.AddRange(link1, link2);
      context.SaveChanges();

      // Act
      var result = await service.DeleteMediaLinkAsync(link1.Id);
      var nullResult = await service.DeleteMediaLinkAsync(Guid.NewGuid());

      // Assert
      Assert.True(result);
      Assert.False(nullResult);

      var mediaLinks = context.MediaLinks.ToList();
      Assert.Single(mediaLinks);
      Assert.Equal(link2.Id, mediaLinks.First().Id);

      var retrievedUser = context.Users.FirstOrDefault(u => u.Id == user.Id);
      Assert.NotNull(retrievedUser);
      Assert.Single(retrievedUser.UploadedMedia);
      Assert.Equal(link2.Id, retrievedUser.UploadedMedia.First().Id);
    }
  }
}
