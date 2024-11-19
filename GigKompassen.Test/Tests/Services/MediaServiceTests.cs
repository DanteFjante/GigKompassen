using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;

namespace GigKompassen.Test.Tests.Services
{
  public class MediaServiceTests
  {
    private readonly MediaService _mediaService;
    private readonly ApplicationDbContext _context;

    private readonly FakeDataHelper f;

    public MediaServiceTests()
    {
      _context = GetInMemoryDbContext();
      _mediaService = new MediaService(_context);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanGetGallery()
    {
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;

      var result = await _mediaService.GetGalleryAsync(gallery.Id);

      Assert.NotNull(result);
      Assert.Equal(gallery, result);
    }

    [Fact]
    public async Task CanGetMediaItem()
    {
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var mediaItem = f.AddFakeMediaItem(gallery.Id)!;

      var result = await _mediaService.GetMediaItemAsync(mediaItem.Id);

      Assert.NotNull(result);
      Assert.Equal(mediaItem, result);
    }

    [Fact]
    public async Task CanGetMediaLink()
    {
      var uploader = f.AddFakeUser();
      var link = f.AddFakeMediaLink(uploader.Id);

      var result = await _mediaService.GetMediaLinkAsync(link.Id);

      Assert.NotNull(result);
      Assert.Equal(link, result);
    }

    [Fact]
    public async Task CanGetMediaLinkFromUserId()
    {
      var uploader = f.AddFakeUser();
      var link = f.AddFakeMediaLink(uploader.Id);
      var link2 = f.AddFakeMediaLink(uploader.Id);

      var result = await _mediaService.GetMediaLinksFromUserAsync(uploader.Id);
      var links = await _context.MediaLinks.ToListAsync();

      Assert.Equal(2, result.Count());
      Assert.Equal(2, links.Count);
    }

    [Fact]
    public async Task CanCreateGallery()
    {
      var owner = f.AddFakeMediaGalleryOwner()!;

      var result = await _mediaService.CreateMediaGalleryAsync(owner.Id, "Test Gallery");
      var galleries = await _context.MediaGalleries.ToListAsync();

      Assert.NotNull(result);
      Assert.Single(galleries);
      Assert.Equal("Test Gallery", result.Name);

    }

    [Fact]
    public async Task CanCreateMediaItem()
    {
      var user = f.AddFakeUser();
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var link = f.AddFakeMediaLink(user.Id);

      var itemDto = GetCreateMediaItemDtos(1).First();

      var result = await _mediaService.CreateMediaItemAsync(gallery.Id, link.Id, itemDto);

      var items = await _context.MediaItems.ToListAsync();

      Assert.NotNull(result);
      Assert.Single(items);
      Assert.Equal(itemDto.Title, result.Title);
      Assert.Equal(itemDto.Description, result.Description);
    }

    [Fact]
    public async Task CanCreateMediaLink()
    {
      var user = f.AddFakeUser()!;

      var linkDto = GetCreateMediaLinkDtos(1).First();

      var result = await _mediaService.CreateMediaLinkAsync(user.Id, linkDto);
      var links = await _context.MediaLinks.ToListAsync();

      Assert.NotNull(result);
      Assert.Single(links);
      Assert.Equal(links[0], result);
      Assert.Equal(linkDto.Path, result.Path);
      Assert.Equal(linkDto.MediaType, result.MediaType);
      Assert.Equal(linkDto.Uploaded, result.Uploaded);
      Assert.Equal(user.Id, result.UploaderId);
    }

    [Fact]
    public async Task CanUpdateMediaItem()
    {
      var user = f.AddFakeUser()!;
      var link = f.AddFakeMediaLink(user.Id)!;
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var item = f.AddFakeMediaItem(gallery.Id, link.Id)!;

      var itemDto = GetUpdateMediaItemDtos(1).First();

      var result = await _mediaService.UpdateMediaItemAsync(item.Id, itemDto);
      var items = await _context.MediaItems.ToListAsync();
      var itemsInGallery = await _context.MediaItems.Where(i => i.GalleryId == gallery.Id).ToListAsync();

      Assert.NotNull(result);
      Assert.Single(items);
      Assert.Single(itemsInGallery);
      Assert.Equal(itemDto.Title, items[0].Title);
      Assert.Equal(itemDto.Description, items[0].Description);
    }

    [Fact]
    public async Task CanDeleteGalleryOwner()
    {
      var user = f.AddFakeUser()!;
      var link = f.AddFakeMediaLink(user.Id)!;
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var item = f.AddFakeMediaItem(gallery.Id, link.Id)!;

      var result = await _mediaService.DeleteMediaGalleryOwnerAsync(owner.Id);
      var owners = await _context.MediaGalleryOwners.ToListAsync();
      var galleries = await _context.MediaGalleries.ToListAsync();
      var items = await _context.MediaItems.ToListAsync();
      var links = await _context.MediaLinks.ToListAsync();

      Assert.True(result);
      Assert.Empty(owners);
      Assert.Empty(galleries);
      Assert.Empty(items);
      Assert.Empty(links);

    }

    [Fact]
    async Task CanDeleteGallery()
    {
      var user = f.AddFakeUser()!;
      var link = f.AddFakeMediaLink(user.Id)!;
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var item = f.AddFakeMediaItem(gallery.Id, link.Id)!;

      var result = await _mediaService.DeleteMediaGalleryAsync(gallery.Id);
      var owners = await _context.MediaGalleryOwners.ToListAsync();
      var galleries = await _context.MediaGalleries.ToListAsync();
      var items = await _context.MediaItems.ToListAsync();
      var links = await _context.MediaLinks.ToListAsync();

      Assert.True(result);
      Assert.Single(owners);
      Assert.Empty(owners!.First()!.Galleries!);
      Assert.Empty(galleries);
      Assert.Empty(items);
      Assert.Empty(links);
    }

    [Fact]
    public async Task CanDeleteMediaItem()
    {
      var user = f.AddFakeUser()!;
      var link = f.AddFakeMediaLink(user.Id)!;
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var item = f.AddFakeMediaItem(gallery.Id, link.Id)!;

      var result = await _mediaService.DeleteMediaItemAsync(item.Id);
      var owners = await _context.MediaGalleryOwners.ToListAsync();
      var galleries = await _context.MediaGalleries.ToListAsync();
      var items = await _context.MediaItems.ToListAsync();
      var links = await _context.MediaLinks.ToListAsync();

      Assert.True(result);
      Assert.Single(owners);
      Assert.Single(owners!.First()!.Galleries!);
      Assert.Single(galleries);
      Assert.Empty(gallery.Items);
      Assert.Empty(items);
      Assert.Empty(links);
    }

    [Fact]
    public async Task CanDeleteMediaLink()
    {
      var user = f.AddFakeUser()!;
      var link = f.AddFakeMediaLink(user.Id)!;
      var owner = f.AddFakeMediaGalleryOwner()!;
      var gallery = f.AddFakeGallery(owner.Id)!;
      var item = f.AddFakeMediaItem(gallery.Id, link.Id)!;

      var result = await _mediaService.DeleteMediaLinkAsync(link.Id);
      var owners = await _context.MediaGalleryOwners.ToListAsync();
      var galleries = await _context.MediaGalleries.ToListAsync();
      var items = await _context.MediaItems.ToListAsync();
      var links = await _context.MediaLinks.ToListAsync();

      Assert.True(result);
      Assert.Single(owners);
      Assert.Single(owners!.First()!.Galleries!);
      Assert.Single(galleries);
      Assert.Single(gallery.Items);
      Assert.Single(items);
      Assert.Null(items.First().MediaLinkId);
      Assert.Empty(links);
    }
  }
}
