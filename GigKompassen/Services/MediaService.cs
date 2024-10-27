using GigKompassen.Data;
using GigKompassen.Dto.Media;
using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Services
{
  public class MediaService
  {

    private readonly ApplicationDbContext _context;

    public MediaService(ApplicationDbContext context)
    {
      _context = context;
    }

    #region Get
    public async Task<MediaGallery?> GetGalleryAsync(Guid galleryId)
    {
      var gallery = await _context.MediaGalleries
          .Include(g => g.Items)
          .Include(g => g.Owner)
          .FirstOrDefaultAsync(g => g.Id == galleryId);

      return gallery;
    }

    public async Task<MediaItem?> GetMediaItemAsync(Guid itemId)
    {
      var item = await _context.MediaItems
          .Include(mi => mi.Link)
          .FirstOrDefaultAsync(mi => mi.Id == itemId);

      return item;
    }

    public async Task<MediaLink?> GetMediaLinkAsync(Guid linkId)
    {
      var link = await _context.MediaLinks
          .FirstOrDefaultAsync(ml => ml.Id == linkId);

      return link;
    }
    #endregion

    #region Create
    public async Task<MediaGalleryOwner> CreateGalleryOwnerAsync(ArtistProfile owner)
    {
      return await CreateGalleryOwnerAsync(owner, null, null);
    }

    public async Task<MediaGalleryOwner> CreateGalleryOwnerAsync(SceneProfile owner)
    {
      return await CreateGalleryOwnerAsync(null, owner, null);
    }

    public async Task<MediaGalleryOwner> CreateGalleryOwnerAsync(Chat owner)
    {
      return await CreateGalleryOwnerAsync(null, null, owner);
    }

    private async Task<MediaGalleryOwner> CreateGalleryOwnerAsync(ArtistProfile? artist, SceneProfile? scene, Chat? chat)
    {
      var gallery = await CreateGalleryAsync();
      MediaGalleryOwner galleryOwner = new MediaGalleryOwner()
      {
        Id = Guid.NewGuid(),
        Gallery = gallery,
      };
      bool hasOwner = false;
      if(artist != null)
      {
        hasOwner = true;
        galleryOwner.ArtistProfile = artist;
        galleryOwner.ArtistProfileId = artist.Id;
      }

      if (scene != null)
      {
        if (hasOwner)
          throw new InvalidOperationException("Only one owner type can be set");
        hasOwner = true;
        galleryOwner.SceneProfile = scene;
        galleryOwner.SceneProfileId = scene.Id;
      }
      if (chat != null)
      {
        if (hasOwner)
          throw new InvalidOperationException("Only one owner type can be set");
        hasOwner = true;
        galleryOwner.Chat = chat;
        galleryOwner.ChatId = chat.Id;
      }
      if(!hasOwner)
      {
        throw new InvalidOperationException("Gallery needs an owner specified");
      }

      var ret = (await _context.MediaGalleryOwners.AddAsync(galleryOwner)).Entity;
      await _context.SaveChangesAsync();
      return ret;
    }

    private async Task<MediaGallery> CreateGalleryAsync()
    {
      MediaGallery gallery = new MediaGallery()
      {
        Id = Guid.NewGuid(),
        Items = new List<MediaItem>()
      };
      var ret = (await _context.MediaGalleries.AddAsync(gallery)).Entity;
      await _context.SaveChangesAsync();
      return ret;
    }

    public async Task<MediaItem> CreateMediaItemAsync(Guid galleryId, Guid linkId, MediaItemDto itemDto)
    {
      if (itemDto == null)
        throw new ArgumentNullException(nameof(itemDto));

      var gallery = await _context.MediaGalleries.FindAsync(galleryId);
      if (gallery == null)
        throw new KeyNotFoundException("MediaGallery not found");

      var link = await _context.MediaLinks.FindAsync(linkId);
      if (link == null)
        throw new KeyNotFoundException("MediaLink not found");

      var mediaItem = new MediaItem
      {
        Id = Guid.NewGuid(),
        Title = itemDto.Title,
        Description = itemDto.Description,
        GalleryId = gallery.Id,
        LinkId = link.Id
      };

      await _context.MediaItems.AddAsync(mediaItem);
      await _context.SaveChangesAsync();

      return mediaItem;
    }

    public async Task<MediaLink> CreateMediaLinkAsync(Guid uploaderId, MediaLinkDto linkDto)
    {
      if (linkDto == null)
        throw new ArgumentNullException(nameof(linkDto));

      var uploader = await _context.Users.FindAsync(uploaderId);
      if (uploader == null)
        throw new KeyNotFoundException("Uploader not found");



      var mediaLink = new MediaLink
      {
        Id = Guid.NewGuid(),
        Path = linkDto.Path,
        MediaType = linkDto.MediaType,
        Uploaded = linkDto.Uploaded ?? DateTime.UtcNow,
        UploaderId = uploader.Id
      };

      await _context.MediaLinks.AddAsync(mediaLink);
      await _context.SaveChangesAsync();

      return mediaLink;
    }
    #endregion

    #region Update
    public async Task<MediaItem> UpdateMediaItemAsync(Guid itemId, MediaItemDto itemDto)
    {
      if (itemDto == null)
        throw new ArgumentNullException(nameof(itemDto));

      var item = await _context.MediaItems.FirstOrDefaultAsync(p => p.Id == itemId);
      if (item == null)
        throw new KeyNotFoundException("MediaItem not found");

      item.Title = itemDto.Title ?? item.Title;
      item.Description = itemDto.Description ?? item.Description;

      await _context.SaveChangesAsync();

      return item;
    }
    #endregion

    #region Delete
    public async Task<bool> DeleteGalleryOwnerAsync(Guid galleryOwnerId)
    {
      var galleryOwner = await _context.MediaGalleryOwners.Include(p => p.Gallery).FirstOrDefaultAsync(p => p.Id == galleryOwnerId);
      if (galleryOwner == null)
        return false;

      var gallery = galleryOwner.Gallery;
      if (gallery != null)
      {
        var items = gallery.Items.ToList();
        if (items != null)
        {
          var links = gallery.Items.Select(i => i.Link).ToList();
          if (links != null)
          {
            _context.MediaLinks.RemoveRange(links);
          }
          _context.MediaItems.RemoveRange(items);
        }
        _context.MediaGalleries.Remove(gallery);
      }

      _context.MediaGalleryOwners.Remove(galleryOwner);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> DeleteGalleryAsync(Guid galleryId)
    {
      var gallery = await _context.MediaGalleries.FirstOrDefaultAsync(p => p.Id == galleryId);
      if (gallery == null)
        return false;

      var items = gallery.Items.ToList();
      if (items != null)
      {
        var links = items.Select(i => i.Link).ToList();
        if (links != null)
        {
          _context.MediaLinks.RemoveRange(links);
        }
        _context.MediaItems.RemoveRange(items);
      }
      _context.MediaGalleries.Remove(gallery);
      await _context.SaveChangesAsync();
      return true;
    }

    public async Task<bool> DeleteMediaItemAsync(Guid itemId)
    {
      var item = await _context.MediaItems.FirstOrDefaultAsync(p => p.Id == itemId);
      if (item == null)
        return false;


      if (item.Link != null)
        _context.MediaLinks.Remove(item.Link);

      _context.MediaItems.Remove(item);
      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> DeleteMediaLinkAsync(Guid linkId)
    {
      var link = await _context.MediaLinks.FindAsync(linkId);
      if (link == null)
        return false;

      _context.MediaLinks.Remove(link);
      await _context.SaveChangesAsync();

      return true;
    }
    #endregion
  }
}
