using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Media;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;

namespace GigKompassen.Services
{
  public class MediaService
  {

    private readonly ApplicationDbContext _context;
    private readonly UserService _userService;

    public event AsyncEventHandler<MediaGallery> OnCreateMediaGallery;
    public event AsyncEventHandler<MediaItem> OnCreateMediaItem;
    public event AsyncEventHandler<MediaLink> OnCreateMediaLink;
    public event AsyncEventHandler<MediaItem> OnUpdateMediaItem;
    public event AsyncEventHandler<MediaGallery> OnDeleteMediaGallery;
    public event AsyncEventHandler<MediaItem> OnDeleteMediaItem;
    public event AsyncEventHandler<MediaLink> OnDeleteMediaLink;

    public MediaService(ApplicationDbContext context, UserService userService)
    {
      _context = context;
      _userService = userService;

      if (_userService != null)
      {
        _userService.OnDeleteUser += async (o, user) =>
        {
          var mediaLinks = await GetMediaLinksFromUserAsync(user.Id);
          await Task.WhenAll(mediaLinks.Select(ml => DeleteMediaLinkAsync(ml.Id)));
        };
      }
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
          .Include(mi => mi.MediaLink)
          .FirstOrDefaultAsync(mi => mi.Id == itemId);

      return item;
    }

    public async Task<MediaLink?> GetMediaLinkAsync(Guid linkId)
    {
      var link = await _context.MediaLinks
          .FirstOrDefaultAsync(ml => ml.Id == linkId);

      return link;
    }

    public async Task<List<MediaLink>> GetMediaLinksFromUserAsync(Guid id)
    {
      if (!_context.Users.Any(u => u.Id == id))
        throw new KeyNotFoundException("User not found");

      return await _context.MediaLinks.Where(ml => ml.UploaderId == id).ToListAsync();
    }
    #endregion

    #region Create
    public async Task<MediaGallery> CreateMediaGalleryAsync(Guid mediaGalleryOwnerId, string name)
    {
      var galleryOwner = await _context.MediaGalleryOwners.Include(mgo => mgo.Galleries).FirstOrDefaultAsync(mgo => mgo.Id == mediaGalleryOwnerId);
      if (galleryOwner == null)
        throw new KeyNotFoundException("MediaGalleryOwner not found");

      MediaGallery gallery = new MediaGallery()
      {
        Id = Guid.NewGuid(),
        Name = name,
        Items = new List<MediaItem>(),
        Owner = galleryOwner,
        OwnerId = galleryOwner.Id
      };

      if(OnCreateMediaGallery != null)
        await OnCreateMediaGallery.InvokeAsync(this, gallery);
      _context.MediaGalleries.Add(gallery);
      galleryOwner.Galleries!.Add(gallery);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create MediaGallery");

      return gallery;
    }

    public async Task<MediaItem> CreateMediaItemAsync(Guid galleryId, Guid linkId, CreateMediaItemDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      var gallery = await _context.MediaGalleries.FindAsync(galleryId);
      if (gallery == null)
        throw new KeyNotFoundException("MediaGallery not found");

      var link = await _context.MediaLinks.FindAsync(linkId);
      if (link == null)
        throw new KeyNotFoundException("MediaLink not found");

      var mediaItem = dto.ToMediaItem();

      mediaItem.MediaLink = link;
      mediaItem.MediaLinkId = link.Id;
      mediaItem.Gallery = gallery;
      mediaItem.GalleryId = gallery.Id;

      if(OnCreateMediaItem != null)
        await OnCreateMediaItem.InvokeAsync(this, mediaItem);

      await _context.MediaItems.AddAsync(mediaItem);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create MediaItem");

      return mediaItem;
    }

    public async Task<MediaLink> CreateMediaLinkAsync(Guid uploaderId, CreateMediaLinkDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      var uploader = await _context.Users.FindAsync(uploaderId);
      if (uploader == null)
        throw new KeyNotFoundException("Uploader not found");

      var mediaLink = dto.ToMediaLink();

      mediaLink.UploaderId = uploader.Id;
      mediaLink.Uploader = uploader;

      if(OnCreateMediaLink != null)
       await OnCreateMediaLink.InvokeAsync(this, mediaLink);

      await _context.MediaLinks.AddAsync(mediaLink);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to create MediaLink");

      return mediaLink;
    }
    #endregion

    #region Update
    public async Task<MediaItem> UpdateMediaItemAsync(Guid itemId, UpdateMediaItemDto dto)
    {
      if (dto == null)
        throw new ArgumentNullException(nameof(dto));

      var item = await _context.MediaItems.FirstOrDefaultAsync(p => p.Id == itemId);
      if (item == null)
        throw new KeyNotFoundException("MediaItem not found");

      dto.UpdateMediaItem(item);

      if(OnUpdateMediaItem != null)
        await OnUpdateMediaItem.InvokeAsync(this, item);

      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to update MediaItem");

      return item;
    }
    #endregion

    #region Delete
    public async Task<bool> DeleteMediaGalleryOwnerAsync(Guid ownerId)
    {
      var owner = await _context.MediaGalleryOwners.Include(mgo => mgo.Galleries).FirstOrDefaultAsync(mgo => mgo.Id == ownerId);
      if (owner == null)
        return false;

      var galleries = owner.Galleries.ToList();
      if (galleries != null)
      {
        var items = galleries.SelectMany(g => g.Items).ToList();
        if (items != null)
        {
          var links = items.Select(i => i.MediaLink).ToList();
          if (links != null)
          {
            _context.MediaLinks.RemoveRange(links);
          }
          _context.MediaItems.RemoveRange(items);
        }
        _context.MediaGalleries.RemoveRange(galleries);
      }
      _context.MediaGalleryOwners.Remove(owner);

      if (await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete MediaGalleryOwner");

      return true;
    }

    public async Task<bool> DeleteMediaGalleryAsync(Guid galleryId)
    {
      var gallery = await _context.MediaGalleries.FirstOrDefaultAsync(p => p.Id == galleryId);
      if (gallery == null)
        return false;

      var items = gallery.Items.ToList();
      if (items != null)
      {
        var links = items.Select(i => i.MediaLink).ToList();
        if (links != null)
        {
          _context.MediaLinks.RemoveRange(links);
        }
        _context.MediaItems.RemoveRange(items);
      }

      if(OnDeleteMediaGallery != null)
        await OnDeleteMediaGallery.InvokeAsync(this, gallery);

      _context.MediaGalleries.Remove(gallery);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete MediaGallery");

      return true;
    }

    public async Task<bool> DeleteMediaItemAsync(Guid itemId)
    {
      var item = await _context.MediaItems.FirstOrDefaultAsync(p => p.Id == itemId);
      if (item == null)
        return false;

      if (item.MediaLink != null)
        _context.MediaLinks.Remove(item.MediaLink);

      if(OnDeleteMediaItem != null)
        await OnDeleteMediaItem.InvokeAsync(this, item);

      _context.MediaItems.Remove(item);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete MediaItem");

      return true;
    }

    public async Task<bool> DeleteMediaLinkAsync(Guid linkId)
    {
      var link = await _context.MediaLinks.FindAsync(linkId);
      if (link == null)
        return false;

      if(OnDeleteMediaLink != null)
        await OnDeleteMediaLink.InvokeAsync(this, link);

      _context.MediaLinks.Remove(link);
      if(await _context.SaveChangesAsync() == 0)
        throw new DbUpdateException("Failed to delete MediaLink");

      return true;
    }
    #endregion
  }

  public record class CreateMediaItemDto(string Title, string? Description)
  {
    public MediaItem ToMediaItem()
    {
      if (string.IsNullOrWhiteSpace(Title))
        throw new ArgumentException("Title is required");

      return new MediaItem()
      {
        Title = Title,
        Description = Description ?? string.Empty
      };
    }

    public static CreateMediaItemDto FromMediaItem(MediaItem mediaItem)
    {
      return new CreateMediaItemDto(mediaItem.Title, mediaItem.Description);
    }
  }

  public record class UpdateMediaItemDto(string? Title = null, string? Description = null)
  {
    public void UpdateMediaItem(MediaItem mediaItem)
    {
      if (!string.IsNullOrWhiteSpace(Title))
        mediaItem.Title = Title;

      if (!string.IsNullOrWhiteSpace(Description))
        mediaItem.Description = Description;
    }

    public static UpdateMediaItemDto FromMediaItem(MediaItem mediaItem)
    {
      return new UpdateMediaItemDto(mediaItem.Title, mediaItem.Description);
    }
  }

  public record class CreateMediaLinkDto(string Path, MediaType MediaType, DateTime? Uploaded)
  {
    public MediaLink ToMediaLink()
    {
      if (string.IsNullOrWhiteSpace(Path))
        throw new ArgumentException("Path is required");

      return new MediaLink()
      {
        Path = Path,
        MediaType = MediaType,
        Uploaded = Uploaded ?? DateTime.UtcNow,
      };
    }

    public static CreateMediaLinkDto FromMediaLink(MediaLink mediaLink)
    {
      return new CreateMediaLinkDto(mediaLink.Path, mediaLink.MediaType, mediaLink.Uploaded);
    }
  }
}
