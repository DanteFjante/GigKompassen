using GigKompassen.Data;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;

using GigKompassen.Models.Media;
using static GigKompassen.Test.Helpers.BogusRepositories;
using GigKompassen.Services;
using GigKompassen.Enums;

namespace GigKompassen.Test.Helpers
{
  public class FakeDataHelper
  {
    private readonly ApplicationDbContext _context;

    public FakeDataHelper(ApplicationDbContext context)
    {
      _context = context;
    }

    public void SetupFakeData()
    {
      for (int i = 0; i < 3; i++)
      {
        var user = AddFakeUser()!;

        AddFakeMediaLink(user.Id);
        AddFakeMediaLink(user.Id);

        for (int j = 0; j < 2; j++)
        {
          var artistProfile = AddFakeArtistProfile(user.Id)!;

          AddFakeGenreToArtist(artistProfile.Id);
          AddFakeGenreToArtist(artistProfile.Id);

          AddFakeArtistMember(artistProfile.Id);
          AddFakeArtistMember(artistProfile.Id);

          var gallery = AddFakeGallery(artistProfile.MediaGalleryOwnerId)!;
          AddFakeMediaItem(gallery.Id);
          AddFakeMediaItem(gallery.Id);

          var sceneProfile = AddFakeSceneProfile(user.Id);

          AddFakeGenreToScene(sceneProfile.Id);
          AddFakeGenreToScene(sceneProfile.Id);

          gallery = AddFakeGallery(artistProfile.MediaGalleryOwnerId)!;
          AddFakeMediaItem(gallery.Id);
          AddFakeMediaItem(gallery.Id);

          AddFakeManagerProfile(user.Id);

          gallery = AddFakeGallery(artistProfile.MediaGalleryOwnerId)!;
          AddFakeMediaItem(gallery.Id);
          AddFakeMediaItem(gallery.Id);
        }
      }
    }

    public ApplicationUser? AddFakeUser(Guid? id = null)
    {
      var user = UserFaker.Generate();
      user.Id = id ?? user.Id;
      _context.Users.Add(user);
      _context.SaveChanges();
      return _context.Users.FirstOrDefault(u => u.Id == user.Id);
    }

    public ArtistProfile? AddFakeArtistProfile(Guid userId, Guid? id = null)
    {
      var user = _context.Users.FirstOrDefault(u => u.Id == userId);
      if (user == null)
      {
        user = AddFakeUser(userId);
      }

      var profile = ArtistFaker.Generate();
      profile.Id = id ?? profile.Id;
      profile.Owner = user;
      profile.OwnerId = user!.Id;

      _context.ArtistProfiles.Add(profile);
      _context.SaveChanges();

      return _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == profile.Id);
    }

    public SceneProfile? AddFakeSceneProfile(Guid userId, Guid? id = null)
    {
      var user = _context.Users.FirstOrDefault(u => u.Id == userId);
      if (user == null)
      {
        user = AddFakeUser(userId);
      }

      var profile = SceneFaker.Generate();
      profile.Id = id ?? profile.Id;
      profile.Owner = user;
      profile.OwnerId = user!.Id;

      _context.SceneProfiles.Add(profile);
      _context.SaveChanges();

      return _context.SceneProfiles.FirstOrDefault(ap => ap.Id == profile.Id);
    }

    public ManagerProfile? AddFakeManagerProfile(Guid userId, Guid? id = null)
    {
      var user = _context.Users.FirstOrDefault(u => u.Id == userId);
      if (user == null)
      {
        user = AddFakeUser(userId);
      }

      var profile = ManagerFaker.Generate();
      profile.Id = id ?? profile.Id;
      profile.Owner = user;
      profile.OwnerId = user!.Id;

      _context.ManagerProfiles.Add(profile);
      _context.SaveChanges();

      return _context.ManagerProfiles.FirstOrDefault(ap => ap.Id == profile.Id);
    }

    public ArtistMember? AddFakeArtistMember(Guid artistId, Guid? id = null)
    {
      var artist = _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == artistId);
      if (artist == null)
      {
        artist = AddFakeArtistProfile(Guid.NewGuid(), artistId);
      }

      var member = ArtistMemberFaker.Generate();
      member.Id = id ?? member.Id;
      member.ArtistProfile = artist;
      member.ArtistProfileId = artist!.Id;

      _context.ArtistMembers.Add(member);
      _context.SaveChanges();

      return _context.ArtistMembers.FirstOrDefault(am => am.Id == member.Id);
    }

    public Genre? AddFakeGenreToArtist(Guid artistId, Guid? id = null)
    {
      var artist = _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == artistId);
      if (artist == null)
      {
        artist = AddFakeArtistProfile(Guid.NewGuid(), artistId);
      }
      var genre = GenreFaker.Generate();
      genre.Id = id ?? genre.Id;
      artist!.Genres!.Add(genre);
      _context.Genres.Add(genre);
      _context.SaveChanges();
      return _context.Genres.FirstOrDefault(g => g.Id == genre.Id);
    }

    public void AddArtistToGenre(Guid artistId, Guid genreId)
    {
      var artist = _context.ArtistProfiles.FirstOrDefault(ap => ap.Id == artistId);
      var genre = _context.Genres.FirstOrDefault(g => g.Id == genreId);
      if (artist == null || genre == null)
      {
        throw new System.Exception("Artist or genre not found");
      }
      artist.Genres!.Add(genre);
      _context.SaveChanges();
    }

    public Genre? AddFakeGenreToScene(Guid sceneId, Guid? id = null)
    {
      var scene = _context.SceneProfiles.FirstOrDefault(ap => ap.Id == sceneId);
      if (scene == null)
      {
        scene = AddFakeSceneProfile(Guid.NewGuid(), sceneId);
      }
      var genre = GenreFaker.Generate();
      genre.Id = id ?? genre.Id;
      scene!.Genres!.Add(genre);
      _context.Genres.Add(genre);
      _context.SaveChanges();
      return _context.Genres.FirstOrDefault(g => g.Id == genre.Id);
    }

    public void AddSceneToGenre(Guid sceneId, Guid genreId)
    {
      var scene = _context.SceneProfiles.FirstOrDefault(ap => ap.Id == sceneId);
      var genre = _context.Genres.FirstOrDefault(g => g.Id == genreId);
      if (scene == null || genre == null)
      {
        throw new System.Exception("Scene or genre not found");
      }
      scene.Genres!.Add(genre);
      _context.SaveChanges();
    }

    public Genre? AddFakeGenre(Guid? genreId = null)
    {
      var genre = GenreFaker.Generate();
      genre.Id = genreId ?? genre.Id;
      _context.Genres.Add(genre);
      _context.SaveChanges();

      return _context.Genres.FirstOrDefault(g => g.Id == genre.Id);
    }

    public MediaGalleryOwner? AddFakeMediaGalleryOwner(Guid? id = null)
    {
      var owner = MediaGalleryOwnerFaker.Generate();
      owner.Id = id ?? owner.Id;

      _context.MediaGalleryOwners.Add(owner);
      _context.SaveChanges();
      return _context.MediaGalleryOwners.FirstOrDefault(o => o.Id == owner.Id);
    }

    public MediaGallery? AddFakeGallery(Guid ownerId, Guid? id = null)
    {
      MediaGalleryOwner? owner = _context.MediaGalleryOwners.FirstOrDefault(o => o.Id == ownerId);
      if (owner == null)
      {
        owner = AddFakeMediaGalleryOwner(ownerId);
      }
      var gallery = MediaGalleryFaker.Generate();
      gallery.Id = id ?? gallery.Id;

      owner.Galleries!.Add(gallery);
      gallery.Owner = owner;
      gallery.OwnerId = owner.Id;

      _context.MediaGalleries.Add(gallery);
      _context.SaveChanges();
      return _context.MediaGalleries.FirstOrDefault(g => g.Id == gallery.Id);
    }

    public MediaItem? AddFakeMediaItem(Guid galleryId, Guid? linkId = null, Guid? id = null)
    {
      MediaGallery? gallery = _context.MediaGalleries.FirstOrDefault(g => g.Id == galleryId);
      if (gallery == null)
      {
        gallery = AddFakeGallery(Guid.NewGuid(), galleryId);
      }

      MediaLink? link = null;
      if (linkId != null)
      {
        link = _context.MediaLinks.FirstOrDefault(l => l.Id == linkId);
      }
      if (link == null)
      {
        link = MediaLinkFaker.Generate();
        link.Id = linkId ?? link.Id;
      }

      var item = MediaItemFaker.Generate();
      item.Id = id ?? item.Id;

      item.Gallery = gallery;
      item.GalleryId = gallery.Id;
      gallery.Items!.Add(item);

      item.MediaLink = link;
      item.MediaLinkId = link.Id;

      _context.MediaItems.Add(item);
      _context.SaveChanges();
      return _context.MediaItems.FirstOrDefault(i => i.Id == item.Id);
    }

    public MediaLink? AddFakeMediaLink(Guid uploaderId, Guid? id = null)
    {
      var uploader = _context.Users.FirstOrDefault(u => u.Id == uploaderId);
      if (uploader == null)
      {
        uploader = AddFakeUser(uploaderId);
      }

      var link = MediaLinkFaker.Generate();
      link.Id = id ?? link.Id;
      link.Uploader = uploader;
      link.UploaderId = uploader!.Id;

      _context.MediaLinks.Add(link);
      _context.SaveChanges();
      return _context.MediaLinks.FirstOrDefault(l => l.Id == link.Id);
    }

    public ProfileAccess? AddFakeProfileAccess(Guid userId, Guid profileId, AccessType? type = null)
    {
      var profile = _context.Profiles.FirstOrDefault(p => p.Id == profileId);
      var user = _context.Users.FirstOrDefault(u => u.Id == userId);

      if (profile == null || user == null)
      {
        throw new System.Exception("Profile or user not found");
      }
      var access = ProfileAccessFaker.Generate();
      access.Profile = profile;
      access.ProfileId = profile.Id;
      access.User = user;
      access.UserId = user.Id;

      access.AccessType = type ?? access.AccessType;

      _context.ProfileAccesses.Add(access);
      _context.SaveChanges();

      return access;
    }

  }
}
