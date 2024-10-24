using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

namespace GigKompassen.Test.Helpers
{
  internal class TestEntityRepository
  {
    private List<ApplicationUser> _users;
    private List<ProfileAccess> _profilesAccesses;

    private List<Chat> _chats;
    private List<ChatParticipant> _chatParticipant;
    private List<ChatMessage> _chatMessages;
    private List<MessageContent> _messageContent;

    private List<MediaGalleryOwner> _mediaGalleryOwners;
    private List<MediaGallery> _mediaGalleries;
    private List<MediaItem> _mediaItems;
    private List<MediaLink> _mediaLinks;

    private List<Profile> _profiles;
    private List<Genre> _genres;
    private List<ArtistMember> _artistMembers;

    public List<ApplicationUser> Users => _users;
    public List<ProfileAccess> ProfilesAccesses => _profilesAccesses;

    public List<Chat> Chats => _chats;
    public List<ChatParticipant> ChatParticipant => _chatParticipant;
    public List<ChatMessage> ChatMessages => _chatMessages;
    public List<MessageContent> MessageContent => _messageContent;
    public List<MessageMediaContent> MessageMediaContent => _messageContent.OfType<MessageMediaContent>().ToList();
    public List<MessageTextContent> MessageTextContent => _messageContent.OfType<MessageTextContent>().ToList();

    public List<MediaGalleryOwner> MediaGalleryOwners => _mediaGalleryOwners;
    public List<MediaGallery> MediaGalleries => _mediaGalleries;
    public List<MediaItem> MediaItems => _mediaItems;
    public List<MediaLink> MediaLinks => _mediaLinks;

    public List<Profile> Profiles => _profiles;
    public List<ArtistProfile> ArtistProfiles => _profiles.OfType<ArtistProfile>().ToList();
    public List<SceneProfile> SceneProfiles => _profiles.OfType<SceneProfile>().ToList();
    public List<ManagerProfile> ManagerProfiles => _profiles.OfType<ManagerProfile>().ToList();

    public List<Genre> Genres => _genres;
    public List<ArtistMember> ArtistMembers => _artistMembers;

    public TestEntityRepository()
    {
      _users = new List<ApplicationUser>();
      _profilesAccesses = new List<ProfileAccess>();
      _chats = new List<Chat>();
      _chatParticipant = new List<ChatParticipant>();
      _chatMessages = new List<ChatMessage>();
      _messageContent = new List<MessageContent>();
      _mediaGalleryOwners = new List<MediaGalleryOwner>();
      _mediaGalleries = new List<MediaGallery>();
      _mediaItems = new List<MediaItem>();
      _mediaLinks = new List<MediaLink>();
      _profiles = new List<Profile>();
      _genres = new List<Genre>();
      _artistMembers = new List<ArtistMember>();
    }

    #region Getters
    public ApplicationUser GetUser(Guid? id = null, List<Profile>? ownedProfiles = null, List<ChatParticipant>? participants = null, List<MediaLink>? mediaLinks = null, List<ProfileAccess>? profileAccesses = null)
    {
      id ??= Guid.NewGuid();

      ApplicationUser user;
      if (_users.Any(u => u.Id == id))
        user = _users.First(u => u.Id == id);
      else
        user = NewUser(id.Value);

      if (ownedProfiles != null)
      {
        user.OwnedProfiles?.AddRange(ownedProfiles);
        ownedProfiles.ForEach(p => p.Owner = user);
        ownedProfiles.ForEach(p => p.OwnerId = user.Id);
      }

      if (participants != null)
      {
        user.ChatParticipations?.AddRange(participants);
        participants.ForEach(p => p.User = user);
        participants.ForEach(p => p.UserId = user.Id);
      }

      if (mediaLinks != null)
      {
        user.UploadedMedia?.AddRange(mediaLinks);
        mediaLinks.ForEach(m => m.Uploader = user);
        mediaLinks.ForEach(m => m.UploaderId = user.Id);
      }

      if (profileAccesses != null)
      {
        user.ProfilesAccesses?.AddRange(profileAccesses);
        profileAccesses.ForEach(p => p.User = user);
        profileAccesses.ForEach(p => p.UserId = user.Id);
      }

      return user;
    }

    public ProfileAccess GetProfileAccess(Guid? id = null, ApplicationUser? user = null, Profile? profile = null)
    {
      id ??= Guid.NewGuid();

      ProfileAccess profileAccess;
      if (_profilesAccesses.Any(pa => pa.Id == id))
        profileAccess = _profilesAccesses.First(pa => pa.Id == id);
      else
        profileAccess = NewProfileAccess(id.Value);

      if(user != null)
      {
        profileAccess.User = user;
        profileAccess.UserId = user.Id;
        user.ProfilesAccesses?.Add(profileAccess);
      }

      if (profile != null)
      {
        profileAccess.Profile = profile;
        profileAccess.ProfileId = profile.Id;
        profile.ProfileAccesses?.Add(profileAccess);
      }

      return profileAccess;
    }

    public Chat GetChat(Guid? id = null, MediaGalleryOwner? owner = null, List<ChatMessage>? messages = null, List<ChatParticipant>? participants = null)
    {
      id ??= Guid.NewGuid();

      Chat chat;
      if (_chats.Any(c => c.Id == id))
        chat = _chats.First(c => c.Id == id);
      else
        chat = NewChat(id.Value);

      if (owner != null) {
        chat.GalleryOwner = owner;
        owner.Chat = chat;
        owner.ChatId = chat.Id;
      }

      if(messages != null)
      {
        chat.Messages?.AddRange(messages);
        messages.ForEach(m => m.Chat = chat);
        messages.ForEach(m => m.ChatId = chat.Id);
      }

      if (participants != null)
      {
        chat.Participants?.AddRange(participants);
        participants.ForEach(p => p.Chat = chat);
        participants.ForEach(p => p.ChatId = chat.Id);
      }

      return chat;
    }

    public ChatParticipant GetParticipant(Guid? id = null, ApplicationUser? user = null, Chat? chat = null, ChatMessage? lastReadId = null, List<ChatMessage>? messages = null)
    {
      id ??= Guid.NewGuid();

      ChatParticipant chatParticipant;
      if (_chatParticipant.Any(cp => cp.Id == id))
        chatParticipant = _chatParticipant.First(cp => cp.Id == id);
      else
        chatParticipant = NewChatParticipant(id.Value);

      if (user != null)
      {
        chatParticipant.User = user;
        chatParticipant.UserId = user.Id;
        user.ChatParticipations?.Add(chatParticipant);
      }

      if (chat != null)
      {
        chatParticipant.Chat = chat;
        chatParticipant.ChatId = chat.Id;
        chat.Participants?.Add(chatParticipant);
      }

      if (lastReadId != null)
      {
        chatParticipant.LastRead = lastReadId;
        chatParticipant.LastReadId = lastReadId.Id;
      }

      if (messages != null)
      {
        chatParticipant.Messages?.AddRange(messages);
        messages.ForEach(m => m.Sender = chatParticipant);
        messages.ForEach(m => m.SenderId = chatParticipant.Id);
      }

      return chatParticipant;
    }

    public ChatMessage GetChatMessage(Guid? id = null, Chat? chat = null, ChatParticipant? sender = null, MessageContent? content = null, ChatMessage? replyTo = null)
    {
      id ??= Guid.NewGuid();

      ChatMessage chatMessage;
      if (_chatMessages.Any(cm => cm.Id == id))
        chatMessage = _chatMessages.First(cm => cm.Id == id);
      else
        chatMessage = NewChatMessage(id.Value);

      if (chat != null)
      {
        chatMessage.Chat = chat;
        chatMessage.ChatId = chat.Id;
        chat.Messages?.Add(chatMessage);
      }

      if (sender != null)
      {
        chatMessage.Sender = sender;
        chatMessage.SenderId = sender.Id;
        sender.Messages?.Add(chatMessage);
      }

      if (content != null)
      {
        chatMessage.Content = content;
        chatMessage.ContentId = content.Id;
      }

      if (replyTo != null)
      {
        chatMessage.ReplyTo = replyTo;
        chatMessage.ReplyToId = replyTo.Id;
      }

      return chatMessage;
    }

    public MessageContent GetMessageContent(Guid? id = null, ChatMessageType chatMessageType = ChatMessageType.Text)
    {
      id ??= Guid.NewGuid();

      MessageContent messageContent;
      if (_messageContent.Any(mc => mc.Id == id))
        messageContent = _messageContent.First(mc => mc.Id == id);
      else
        messageContent = chatMessageType switch
        {
          ChatMessageType.Media => GetMessageMediaContent(id),
          ChatMessageType.Text => GetMessageTextContent(id),
          _ => throw new NotImplementedException()
        };

      return messageContent;
    }

    public MessageMediaContent GetMessageMediaContent(Guid? id = null, MediaLink? mediaLink = null)
    {
      id ??= Guid.NewGuid();

      MessageMediaContent messageMediaContent;
      if (_messageContent.OfType<MessageMediaContent>().Any(mc => mc.Id == id))
        messageMediaContent = _messageContent.OfType<MessageMediaContent>().First(mc => mc.Id == id);
      else
        messageMediaContent = NewMessageMediaContent(id.Value);

      if(mediaLink != null)
      {
        messageMediaContent.MediaLink = mediaLink;
        messageMediaContent.MediaLinkId = mediaLink.Id;
      }

      return messageMediaContent;
    }

    public MessageTextContent GetMessageTextContent(Guid? id = null)
    {
      id ??= Guid.NewGuid();

      MessageTextContent messageTextContent;
      if (_messageContent.OfType<MessageTextContent>().Any(mc => mc.Id == id))
        messageTextContent = _messageContent.OfType<MessageTextContent>().First(mc => mc.Id == id);
      else
        messageTextContent = NewMessageTextContent(id.Value);

      return messageTextContent;
    }

    public MediaGalleryOwner GetMediaGalleryOwner(Guid? id = null, Chat? chat = null, ArtistProfile? artistProfile = null, SceneProfile? sceneProfile = null, MediaGallery? gallery = null)
    {
      id ??= Guid.NewGuid();

      MediaGalleryOwner mediaGalleryOwner;
      if (_mediaGalleryOwners.Any(mgo => mgo.Id == id))
        mediaGalleryOwner = _mediaGalleryOwners.First(mgo => mgo.Id == id);
      else
        mediaGalleryOwner = NewMediaGalleryOwner(id.Value);

      if (chat != null)
      {
        mediaGalleryOwner.Chat = chat;
        mediaGalleryOwner.ChatId = chat.Id;
        chat.GalleryOwner = mediaGalleryOwner;
      }

      if (artistProfile != null)
      {
        mediaGalleryOwner.ArtistProfile = artistProfile;
        mediaGalleryOwner.ArtistProfileId = artistProfile.Id;
        artistProfile.GalleryOwner = mediaGalleryOwner;
      }

      if (sceneProfile != null)
      {
        mediaGalleryOwner.SceneProfile = sceneProfile;
        mediaGalleryOwner.SceneProfileId = sceneProfile.Id;
        sceneProfile.GalleryOwner = mediaGalleryOwner;
      }

      if (gallery != null)
      {
        mediaGalleryOwner.Gallery = gallery;
        gallery.Owner = mediaGalleryOwner;
        gallery.OwnerId = mediaGalleryOwner.Id;
      }

      return mediaGalleryOwner;

    }

    public MediaGallery GetMediaGallery(Guid? id = null, MediaGalleryOwner? owner = null, List<MediaItem>? items = null)
    {
      id ??= Guid.NewGuid();

      MediaGallery mediaGallery;
      if (_mediaGalleries.Any(mg => mg.Id == id))
        mediaGallery = _mediaGalleries.First(mg => mg.Id == id);
      else
        mediaGallery = NewMediaGallery(id.Value);

      if (owner != null)
      {
        mediaGallery.Owner = owner;
        mediaGallery.OwnerId = owner.Id;
        owner.Gallery = mediaGallery;
      }

      if (items != null)
      {
        mediaGallery.Items?.AddRange(items);
        items.ForEach(i => i.Gallery = mediaGallery);
        items.ForEach(i => i.GalleryId = mediaGallery.Id);
      }

      return mediaGallery;
    }

    public MediaItem GetMediaItem(Guid? id = null, MediaGallery? gallery = null, MediaLink? link = null)
    {
      id ??= Guid.NewGuid();

      MediaItem mediaItem;
      if (_mediaItems.Any(mi => mi.Id == id))
        return _mediaItems.First(mi => mi.Id == id);
      else
        mediaItem = NewMediaItem(id.Value);

      if (gallery != null)
      {
        mediaItem.Gallery = gallery;
        mediaItem.GalleryId = gallery.Id;
        gallery.Items.Add(mediaItem);
      }

      if (link != null)
      {
        mediaItem.Link = link;
      }

      return mediaItem;
    }

    public MediaLink GetMediaLink(Guid? id = null, ApplicationUser? uploader = null)
    {
      id ??= Guid.NewGuid();

      MediaLink mediaLink;
      if (_mediaLinks.Any(ml => ml.Id == id))
        mediaLink = _mediaLinks.First(ml => ml.Id == id);
      else
        mediaLink = NewMediaLink(id.Value);

      if (uploader != null)
      {
        mediaLink.Uploader = uploader;
        mediaLink.UploaderId = uploader.Id;
        uploader.UploadedMedia?.Add(mediaLink);
      }

      return mediaLink;
    }

    public Profile GetProfile(Guid? id = null, List<ProfileAccess>? profileAccesses = null, ApplicationUser? owner = null, ProfileTypes type = ProfileTypes.Manager)
    {
      id ??= Guid.NewGuid();

      Profile profile;
      if (_profiles.Any(p => p.Id == id))
        profile = _profiles.First(p => p.Id == id);
      else
        profile = type switch
        {
          ProfileTypes.Artist => GetArtistProfile(id, owner: owner, userAccessPermissions: profileAccesses),
          ProfileTypes.Scene => GetSceneProfile(id, owner: owner, userAccessPermissions: profileAccesses),
          ProfileTypes.Manager => GetManagerProfile(id, owner: owner, userAccessPermissions: profileAccesses),
          _ => throw new NotImplementedException()
        };

      return profile;
    }

    public Genre GetGenre(Guid? id = null, List<ArtistProfile>? artistProfiles = null, List<SceneProfile>? sceneProfiles = null)
    {
      id ??= Guid.NewGuid();

      Genre genre;
      if (_genres.Any(g => g.Id == id))
        genre = _genres.First(g => g.Id == id);
      else
        genre = NewGenre(id.Value);

      if (artistProfiles != null)
      {
        genre.ArtistProfiles?.AddRange(artistProfiles);
        artistProfiles.ForEach(ap => ap.Genres?.Add(genre));
      }

      if (sceneProfiles != null)
      {
        genre.SceneProfiles?.AddRange(sceneProfiles);
        sceneProfiles.ForEach(sp => sp.Genres?.Add(genre));
      }

      return genre;
    }

    public ArtistProfile GetArtistProfile(Guid? id = null, MediaGalleryOwner? galleryOwner = null, ApplicationUser? owner = null, List<ArtistMember>? members = null, List<Genre>? genres = null, List<ProfileAccess>? userAccessPermissions = null)
    {
      id ??= Guid.NewGuid();

      ArtistProfile artistProfile;
      if (_profiles.OfType<ArtistProfile>().Any(ap => ap.Id == id))
        artistProfile = _profiles.OfType<ArtistProfile>().First(ap => ap.Id == id);
      else
        artistProfile = NewArtistProfile(id.Value);

      if (galleryOwner != null)
      {
        artistProfile.GalleryOwner = galleryOwner;
        galleryOwner.ArtistProfile = artistProfile;
        galleryOwner.ArtistProfileId = artistProfile.Id;
      }

      if (owner != null)
      {
        artistProfile.Owner = owner;
        artistProfile.OwnerId = owner.Id;
        owner.OwnedProfiles?.Add(artistProfile);
      }

      if (members != null)
      {
        artistProfile.Members?.AddRange(members);
        members.ForEach(m => m.ArtistProfile = artistProfile);
        members.ForEach(m => m.ArtistProfileId = artistProfile.Id);
      }

      if (genres != null)
      {
        artistProfile.Genres?.AddRange(genres);
        genres.ForEach(g => g.ArtistProfiles?.Add(artistProfile));
      }

      if (userAccessPermissions != null)
      {
        artistProfile.ProfileAccesses?.AddRange(userAccessPermissions);
        userAccessPermissions.ForEach(p => p.Profile = artistProfile);
        userAccessPermissions.ForEach(p => p.ProfileId = artistProfile.Id);
      }

      return artistProfile;
    }

    public ArtistMember GetArtistMember(Guid? id = null, ArtistProfile? artistProfile = null)
    {
      id ??= Guid.NewGuid();

      ArtistMember artistMember;
      if (_artistMembers.Any(am => am.Id == id))
        artistMember = _artistMembers.First(am => am.Id == id);
      else
        artistMember = NewArtistMember(id.Value);

      if (artistProfile != null)
      {
        artistMember.ArtistProfile = artistProfile;
        artistMember.ArtistProfileId = artistProfile.Id;
        artistProfile.Members?.Add(artistMember);
      }

      return artistMember;
    }

    public SceneProfile GetSceneProfile(Guid? id = null, MediaGalleryOwner? galleryOwner = null, ApplicationUser? owner = null, List<Genre>? genres = null, List<ProfileAccess>? userAccessPermissions = null)
    {
      id ??= Guid.NewGuid();
      int count = _profiles.OfType<SceneProfile>().Count();

      SceneProfile sceneProfile;
      if (_profiles.OfType<SceneProfile>().Any(sp => sp.Id == id))
        sceneProfile = _profiles.OfType<SceneProfile>().First(sp => sp.Id == id);
      else
        sceneProfile = NewSceneProfile(id.Value);

      if(galleryOwner != null)
      {
        sceneProfile.GalleryOwner = galleryOwner;
        galleryOwner.SceneProfile = sceneProfile;
        galleryOwner.SceneProfileId = sceneProfile.Id;
      }

      if (owner != null)
      {
        sceneProfile.Owner = owner;
        sceneProfile.OwnerId = owner.Id;
        owner.OwnedProfiles?.Add(sceneProfile);
      }

      if (genres != null)
      {
        sceneProfile.Genres?.AddRange(genres);
        genres.ForEach(g => g.SceneProfiles?.Add(sceneProfile));
      }

      if(userAccessPermissions != null)
      {
        sceneProfile.ProfileAccesses?.AddRange(userAccessPermissions);
        userAccessPermissions.ForEach(p => p.Profile = sceneProfile);
        userAccessPermissions.ForEach(p => p.ProfileId = sceneProfile.Id);
      }

      return sceneProfile;
    }

    public ManagerProfile GetManagerProfile(Guid? id = null, ApplicationUser? owner = null, List<ProfileAccess>? userAccessPermissions = null)
    {
      id ??= Guid.NewGuid();

      ManagerProfile managerProfile;
      if (_profiles.OfType<ManagerProfile>().Any(mp => mp.Id == id))
        managerProfile = _profiles.OfType<ManagerProfile>().First(mp => mp.Id == id);
      else
        managerProfile = NewManagerProfile(id.Value);

      if (owner != null)
      {
        managerProfile.Owner = owner;
        managerProfile.OwnerId = owner.Id;
        owner.OwnedProfiles?.Add(managerProfile);
      }

      if (userAccessPermissions != null)
      {
        managerProfile.ProfileAccesses?.AddRange(userAccessPermissions);
        userAccessPermissions.ForEach(p => p.Profile = managerProfile);
        userAccessPermissions.ForEach(p => p.ProfileId = managerProfile.Id);
      }

      return managerProfile;
    }
    #endregion

    #region Creaters

    public ApplicationUser NewUser(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _users.Count;
      var user = new ApplicationUser()
      {
        Id = id.Value,
        AccessFailedCount = 0,
        Email = $"test{count}@test.com",
        EmailConfirmed = true,
        LockoutEnabled = false,
        UserName = $"test{count}",
        ConcurrencyStamp = Guid.NewGuid().ToString(),
        TwoFactorEnabled = false,
        PhoneNumberConfirmed = false,
        Created = DateTime.UtcNow,
        ProfileCompleted = false,
        FirstName = $"First name test{count}",
        LastName = $"Last name test{count}",
        LastLogin = null,
        PhoneNumber = null,
        SecurityStamp = Guid.NewGuid().ToString(),
        LockoutEnd = null,
        PasswordHash = Guid.NewGuid().ToString(),
        ChatParticipations = new List<ChatParticipant>(),
        ProfilesAccesses = new List<ProfileAccess>(),
        UploadedMedia = new List<MediaLink>(),
      };
      _users.Add(user);
      return user;
    }

    public ProfileAccess NewProfileAccess(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      var profileAccess = new ProfileAccess()
      {
        Id = id.Value,
        AccessType = AccessType.View,
      };

      _profilesAccesses.Add(profileAccess);
      return profileAccess;
    }

    public Chat NewChat(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _chats.Count;
      var chat = new Chat()
      {
        Id = id.Value,
        Name = $"Test Chat {count}",
        Created = DateTime.UtcNow,
        Messages = new List<ChatMessage>(),
        Participants = new List<ChatParticipant>(),
      };

      _chats.Add(chat);
      return chat;
    }

    public ChatParticipant NewChatParticipant(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      var chatParticipant = new ChatParticipant()
      {
        Id = id.Value,
        LastReadId = null,
        Messages = new List<ChatMessage>(),
      };

      _chatParticipant.Add(chatParticipant);
      return chatParticipant;
    }

    public ChatMessage NewChatMessage(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      var chatMessage = new ChatMessage()
      {
        Id = id.Value,
        Sent = DateTime.UtcNow,
        ReplyToId = null,
      };

      _chatMessages.Add(chatMessage);
      return chatMessage;
    }

    public MessageTextContent NewMessageTextContent(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _messageContent.OfType<MessageTextContent>().Count();
      var textContent = new MessageTextContent()
      {
        Id = id.Value,
        Created = DateTime.UtcNow,
        Text = $"Test message content {count}",
      };

      _messageContent.Add(textContent);
      return textContent;
    }

    public MessageMediaContent NewMessageMediaContent(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _messageContent.OfType<MessageMediaContent>().Count();
      var mediaContent = new MessageMediaContent()
      {
        Id = id.Value,
        Created = DateTime.UtcNow,
        Comment = $"Test message content {count}",
      };

      _messageContent.Add(mediaContent);
      return mediaContent;
    }

    public MediaGalleryOwner NewMediaGalleryOwner(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      var mediaGalleryOwner = new MediaGalleryOwner()
      {
        Id = id.Value,
      };

      _mediaGalleryOwners.Add(mediaGalleryOwner);
      return mediaGalleryOwner;
    }

    public MediaGallery NewMediaGallery(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      var mediaGallery = new MediaGallery()
      {
        Id = id.Value,
        Items = new List<MediaItem>(),
      };

      _mediaGalleries.Add(mediaGallery);
      return mediaGallery;
    }

    public MediaItem NewMediaItem(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _mediaItems.Count;
      string name = $"Test Media Item {count}";
      var mediaItem = new MediaItem()
      {
        Id = id.Value,
        Title = name,
        Description = $"Test Description of {name}",
      };

      _mediaItems.Add(mediaItem);
      return mediaItem;
    }

    public MediaLink NewMediaLink(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _mediaLinks.Count;
      var mediaLink = new MediaLink()
      {
        Id = id.Value,
        Path = $"Test/Path/of/media/link/{count}",
        MediaType = MediaType.Image,
        Uploaded = DateTime.UtcNow,
      };

      _mediaLinks.Add(mediaLink);
      return mediaLink;
    }

    public ArtistProfile NewArtistProfile(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _profiles.OfType<ArtistProfile>().Count();
      string name = $"Test Artist {count}";
      var artistProfile = new ArtistProfile()
      {
        Id = id.Value,
        Name = name,
        Location = $"Location of {name}",
        Bio = $"Bio of {name}",
        Description = $"Description of {name}",
        Availability = AvailabilityStatus.Open,
        Public = true,
        Genres = new List<Genre>(),
        Members = new List<ArtistMember>(),
        ProfileAccesses = new List<ProfileAccess>(),
      };

      _profiles.Add(artistProfile);
      return artistProfile;
    }

    public ArtistMember NewArtistMember(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _artistMembers.Count;
      string name = $"Test Member {count}";
      var artistMember = new ArtistMember()
      {
        Id = id.Value,
        Name = name,
        Role = $"Test Role of {name}",

      };

      _artistMembers.Add(artistMember);
      return artistMember;
    }

    public SceneProfile NewSceneProfile(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _profiles.OfType<SceneProfile>().Count();
      string name = $"Test Scene {count}";
      var sceneProfile = new SceneProfile()
      {
        Id = id.Value,
        Name = name,
        Address = $"Address of {name}",
        Amenities = $"Amenities of {name}",
        Bio = $"Bio of {name}",
        VenueType = $"Venue Type of {name}",
        ContactInfo = $"Contact Info of {name}",
        Description = $"Description of {name}",
        OpeningHours = $"Opening Hours of {name}",
        Capacity = 0,
        Public = true,
        Genres = new List<Genre>(),
        ProfileAccesses = new List<ProfileAccess>(),
      };

      _profiles.Add(sceneProfile);
      return sceneProfile;
    }

    public ManagerProfile NewManagerProfile(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _profiles.OfType<ManagerProfile>().Count();
      string name = $"Test Manager {count}";
      var managerProfile = new ManagerProfile()
      {
        Id = id.Value,
        Name = name,
        Description = $"Test Description of {name}",
        Location = $"Test Location of {name}",
        ProfileAccesses = new List<ProfileAccess>(),
      };

      _profiles.Add(managerProfile);
      return managerProfile;
    }

    public Genre NewGenre(Guid? id = null)
    {
      id ??= Guid.NewGuid();
      int count = _genres.Count;
      var genre = new Genre()
      {
        Id = id.Value,
        Name = $"Test Genre {count}",
        ArtistProfiles = new List<ArtistProfile>(),
        SceneProfiles = new List<SceneProfile>(),
      };

      _genres.Add(genre);
      return genre;
    }
    #endregion
  }
}
