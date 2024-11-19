using Bogus;

using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Chats;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;

namespace GigKompassen.Test.Helpers
{
  public static class BogusRepositories
  {
    public static Faker<ApplicationUser> UserFaker = new Faker<ApplicationUser>()
      .RuleFor(u => u.Id, f => f.Random.Guid())
      .RuleFor(u => u.Email, f => f.Person.Email)
      .RuleFor(u => u.UserName, f => f.Person.Email)
      .RuleFor(u => u.NormalizedUserName, (f, u) => u.UserName?.ToUpper())
      .RuleFor(u => u.NormalizedEmail, (f, u) => u.Email?.ToUpper())
      .RuleFor(u => u.FirstName, f => f.Person.FirstName)
      .RuleFor(u => u.LastName, f => f.Person.LastName)
      .RuleFor(u => u.RegistrationCompleted, f => f.Random.Bool())
      .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool())
      .RuleFor(u => u.Created, f => f.Date.Past())
      .RuleFor(u => u.LastLogin, f => f.Date.Past())
      .RuleFor(u => u.SecurityStamp, f => f.Random.Guid().ToString());

    public static Faker<ManagerProfile> ManagerFaker = new Faker<ManagerProfile>()
      .RuleFor(m => m.Id, f => f.Random.Guid())
      .RuleFor(m => m.Name, f => f.Company.CompanyName())
      .RuleFor(m => m.Description, f => f.Lorem.Sentence())
      .RuleFor(m => m.Location, f => f.Address.City())
      .RuleFor(m => m.MediaGalleryOwnerId, f => f.Random.Guid())
      .RuleFor(m => m.MediaGalleryOwner, (f, m) => new MediaGalleryOwner()
      {
        Galleries = new(),
        Id = m.MediaGalleryOwnerId
      });

    public static Faker<SceneProfile> SceneFaker = new Faker<SceneProfile>()
      .RuleFor(s => s.Id, f => f.Random.Guid())
      .RuleFor(s => s.Name, f => f.Company.CompanyName())
      .RuleFor(s => s.Address, f => f.Address.FullAddress())
      .RuleFor(s => s.Bio, f => f.Lorem.Sentence())
      .RuleFor(s => s.Capacity, f => f.Random.Number(50, 5000))
      .RuleFor(s => s.ContactInfo, f => f.Phone.PhoneNumber())
      .RuleFor(s => s.Amenities, f => f.Lorem.Sentence())
      .RuleFor(s => s.Description, f => f.Lorem.Sentence())
      .RuleFor(s => s.OpeningHours, f => f.Lorem.Sentence())
      .RuleFor(s => s.VenueType, f => f.Lorem.Word())
      .RuleFor(s => s.MediaGalleryOwnerId, f => f.Random.Guid())
      .RuleFor(s => s.MediaGalleryOwner, (f, s) => new MediaGalleryOwner()
      {
        Galleries = new(),
        Id = s.MediaGalleryOwnerId
      });

    public static Faker<ArtistProfile> ArtistFaker = new Faker<ArtistProfile>()
      .RuleFor(a => a.Id, f => f.Random.Guid())
      .RuleFor(a => a.Name, f => f.Person.UserName)
      .RuleFor(a => a.Bio, f => f.Lorem.Sentence())
      .RuleFor(a => a.Description, f => f.Lorem.Sentence())
      .RuleFor(a => a.Availability, f => f.PickRandom<AvailabilityStatus>())
      .RuleFor(a => a.MediaGalleryOwnerId, f => f.Random.Guid())
      .RuleFor(a => a.MediaGalleryOwner, (f, a) => new MediaGalleryOwner() 
      { 
        Galleries = new(), 
        Id = a.MediaGalleryOwnerId
      });

    public static Faker<ArtistMember> ArtistMemberFaker = new Faker<ArtistMember>()
      .RuleFor(am => am.Id, f => f.Random.Guid())
      .RuleFor(am => am.Name, f => f.Person.FullName)
      .RuleFor(am => am.Role, f => f.Name.JobTitle());

    public static Faker<Genre> GenreFaker = new Faker<Genre>()
      .StrictMode(true)
      .RuleFor(g => g.Id, f => f.Random.Guid())
      .RuleFor(g => g.Name, f => GetGenreName(f));

    public static Faker<MediaGalleryOwner> MediaGalleryOwnerFaker = new Faker<MediaGalleryOwner>()
      .RuleFor(mgo => mgo.Id, f => f.Random.Guid())
      .RuleFor(mgo => mgo.Galleries, f => new());

    public static Faker<MediaGallery> MediaGalleryFaker = new Faker<MediaGallery>()
      .RuleFor(m => m.Id, f => f.Random.Guid())
      .RuleFor(m => m.Name, f => f.Lorem.Word())
      .RuleFor(m => m.Items, f => new());

    public static Faker<MediaItem> MediaItemFaker = new Faker<MediaItem>()
      .RuleFor(mi => mi.Id, f => f.Random.Guid())
      .RuleFor(mi => mi.Title, f => f.Lorem.Word());

    public static Faker<MediaLink> MediaLinkFaker = new Faker<MediaLink>()
      .RuleFor(ml => ml.Id, f => f.Random.Guid())
      .RuleFor(ml => ml.MediaType, f => f.PickRandom<MediaType>())
      .RuleFor(ml => ml.Path, f => f.Internet.Url());

    public static Faker<Chat> ChatFaker = new Faker<Chat>()
      .RuleFor(c => c.Id, f => f.Random.Guid())
      .RuleFor(c => c.Name, f => f.Lorem.Word())
      .RuleFor(c => c.Created, f => f.Date.Past());

    public static Faker<ChatMessage> ChatMessageFaker = new Faker<ChatMessage>()
      .RuleFor(cm => cm.Id, f => f.Random.Guid())
      .RuleFor(cm => cm.Sent, f => f.Date.Past());

    public static Faker<ChatParticipant> ChatParticipantFaker = new Faker<ChatParticipant>()
      .RuleFor(cp => cp.Id, f => f.Random.Guid())
      .RuleFor(cp => cp.LastReadTime, f => f.Date.Past());

    public static Faker<MessageMediaContent> MessageMediaContentFaker = new Faker<MessageMediaContent>()
      .RuleFor(mmc => mmc.Id, f => f.Random.Guid())
      .RuleFor(mmc => mmc.Created, f => f.Date.Past())
      .RuleFor(mmc => mmc.Comment, f => f.Lorem.Sentence());

    public static Faker<MessageTextContent> MessageTextContentFaker = new Faker<MessageTextContent>()
      .RuleFor(mmc => mmc.Id, f => f.Random.Guid())
      .RuleFor(mmc => mmc.Created, f => f.Date.Past())
      .RuleFor(mmc => mmc.Text, f => f.Lorem.Sentence());

    public static Faker<ProfileAccess> ProfileAccessFaker = new Faker<ProfileAccess>()
      .RuleFor(pa => pa.Id, f => f.Random.Guid())
      .RuleFor(pa => pa.AccessType, f => f.PickRandom<AccessType>());


    public static IEnumerable<CreateArtistDto> GetCreateArtistDtos(int capacity = 1)
    {
      return BogusRepositories.ArtistFaker.Generate(capacity).Select(CreateArtistDto.FromArtistProfile);
    }

    public static IEnumerable<UpdateArtistDto> GetUpdateArtistDtos(int capacity = 1)
    {
      return BogusRepositories.ArtistFaker.Generate(capacity).Select(UpdateArtistDto.FromArtistProfile);
    }

    public static IEnumerable<ArtistMemberDto> GetArtistMemberDtos(int capacity = 1)
    {
      return BogusRepositories.ArtistMemberFaker.Generate(capacity).Select(ArtistMemberDto.FromArtistMember);
    }

    public static IEnumerable<CreateSceneDto> GetCreateSceneDtos(int capacity = 1)
    {
      return BogusRepositories.SceneFaker.Generate(capacity).Select(CreateSceneDto.FromSceneProfile);
    }

    public static IEnumerable<UpdateSceneDto> GetUpdateSceneDtos(int capacity = 1)
    {
      return BogusRepositories.SceneFaker.Generate(capacity).Select(UpdateSceneDto.FromSceneProfile);
    }

    public static IEnumerable<CreateManagerDto> GetCreateManagerDtos(int capacity = 1)
    {
      return BogusRepositories.ManagerFaker.Generate(capacity).Select(CreateManagerDto.FromManagerProfile);
    }

    public static IEnumerable<UpdateManagerDto> GetUpdateManagerDtos(int capacity = 1)
    {
      return BogusRepositories.ManagerFaker.Generate(capacity).Select(UpdateManagerDto.FromManagerProfile);
    }

    public static IEnumerable<string> GetGenreNames(int capacity = 1)
    {
      return BogusRepositories.GenreFaker.Generate(capacity).Select(g => g.Name);
    }

    public static IEnumerable<CreateMediaItemDto> GetCreateMediaItemDtos(int capacity = 1)
    {
      return BogusRepositories.MediaItemFaker.Generate(capacity).Select(CreateMediaItemDto.FromMediaItem);
    }

    public static IEnumerable<UpdateMediaItemDto> GetUpdateMediaItemDtos(int capacity = 1)
    {
      return BogusRepositories.MediaItemFaker.Generate(capacity).Select(UpdateMediaItemDto.FromMediaItem);
    }

    public static IEnumerable<CreateMediaLinkDto> GetCreateMediaLinkDtos(int capacity = 1)
    {
      return BogusRepositories.MediaLinkFaker.Generate(capacity).Select(CreateMediaLinkDto.FromMediaLink);
    }


    private static string[] GenreNames = new[]
    {   
      "Rock", "Jazz", "Pop", "Hip-Hop", "Classical", "Electronic", "Blues", "Reggae", "Country", "Folk",
      "R&B", "Soul", "Heavy Metal", "Punk", "Disco", "Ambient",  "Funk", "Gospel", "Ska", "House", "Techno",
      "Indie Rock", "Synthwave", "Alternative", "Latin", "K-Pop", "Grunge", "Opera", "Trap", "Dancehall",
    };

    private static List<string> GenreNamesLeft = GenreNames!.ToList();
    private static string GetGenreName(Faker f)
    {
      if (GenreNamesLeft.Count == 0)
      {
        return "Genre " + f.UniqueIndex;
      }
      string genreName = f.PickRandom(GenreNamesLeft);
      GenreNamesLeft.Remove(genreName);
      return genreName;
    }
  }
}
