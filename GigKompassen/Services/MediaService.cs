using GigKompassen.Data;
using GigKompassen.Dto.Media;
using GigKompassen.Models.Accounts;
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

    public async Task<MediaGalleryOwner> CreateGalleryAsync(ArtistProfile owner)
    {
      return await CreateGalleryAsync<ArtistProfile>(owner);
    }

    public async Task<MediaGalleryOwner> CreateGalleryAsync(SceneProfile owner)
    {
      return await CreateGalleryAsync<SceneProfile>(owner);
    }

    public async Task<MediaGalleryOwner> CreateGalleryAsync(Chat owner)
    {
      return await CreateGalleryAsync<Chat>(owner);
    }

    private async Task<MediaGalleryOwner> CreateGalleryAsync<T>(T owner) where T : class
    {
      var gallery = await CreateGalleryAsync();
      MediaGalleryOwner galleryOwner = new MediaGalleryOwner()
      {
        Id = Guid.NewGuid(),
        Gallery = gallery,
        

      };

      if(owner is ArtistProfile artist)
      {
        galleryOwner.ArtistProfile = artist;
        galleryOwner.ArtistProfileId = artist.Id;
      }
      else if (owner is SceneProfile scene)
      {
        galleryOwner.SceneProfile = scene;
        galleryOwner.SceneProfileId = scene.Id;
      }
      else if (owner is Chat chat)
      {
        galleryOwner.Chat = chat;
        galleryOwner.ChatId = chat.Id;
      }
      else
      {
        throw new InvalidOperationException("Unsupported owner type.");
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

    public async Task<MediaItem> CreateItemAsync(Guid galleryId, Guid linkId, MediaItemDto itemDto)
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

    public async Task<MediaLink> CreateLinkAsync(Guid uploaderId, MediaLinkDto linkDto)
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

    public async Task<MediaGallery> GetGalleryAsync(Guid galleryId)
    {
      var gallery = await _context.MediaGalleries
          .Include(g => g.Items)
          .FirstOrDefaultAsync(g => g.Id == galleryId);

      if (gallery == null)
        throw new KeyNotFoundException("MediaGallery not found");

      return gallery;
    }

    public async Task<MediaItem> GetMediaItemAsync(Guid itemId)
    {
      var item = await _context.MediaItems
          .Include(mi => mi.Link)
          .FirstOrDefaultAsync(mi => mi.Id == itemId);

      if (item == null)
        throw new KeyNotFoundException("MediaItem not found");

      return item;
    }

    public async Task<MediaLink> GetMediaLinkAsync(Guid linkId)
    {
      var link = await _context.MediaLinks
          .FirstOrDefaultAsync(ml => ml.Id == linkId);

      if (link == null)
        throw new KeyNotFoundException("MediaLink not found");

      return link;
    }

    public async Task<MediaItem> UpdateMediaItemAsync(Guid itemId, MediaItemDto itemDto)
    {
      if (itemDto == null)
        throw new ArgumentNullException(nameof(itemDto));

      var item = await _context.MediaItems.FindAsync(itemId);
      if (item == null)
        throw new KeyNotFoundException("MediaItem not found");

      item.Title = itemDto.Title ?? item.Title;
      item.Description = itemDto.Description ?? item.Description;

      await _context.SaveChangesAsync();

      return item;
    }

    public async Task<MediaLink> UpdateMediaLinkAsync(Guid linkId, MediaLinkDto linkDto)
    {
      if (linkDto == null)
        throw new ArgumentNullException(nameof(linkDto));

      var link = await _context.MediaLinks.FindAsync(linkId);
      if (link == null)
        throw new KeyNotFoundException("MediaLink not found");

      link.Path = linkDto.Path ?? link.Path;
      link.MediaType = linkDto.MediaType;

      await _context.SaveChangesAsync();

      return link;
    }

    public async Task<bool> DeleteGalleryAsync(Guid galleryId)
    {
      var gallery = await _context.MediaGalleries.FindAsync(galleryId);
      if (gallery == null)
        return false;

      _context.MediaGalleries.Remove(gallery);
      await _context.SaveChangesAsync();

      return true;
    }

    public async Task<bool> DeleteMediaItemAsync(Guid itemId)
    {
      var item = await _context.MediaItems.FindAsync(itemId);
      if (item == null)
        return false;

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
  }
}
