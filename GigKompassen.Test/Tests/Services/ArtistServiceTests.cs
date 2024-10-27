using GigKompassen.Dto.Profiles;
using GigKompassen.Enums;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;

namespace GigKompassen.Test.Tests.Services
{
  public class ArtistServiceTests
  {

    [Fact]
    public async Task CanGetAllArtistProfiles()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var genre3 = repo.NewGenre();
      
      var member1 = repo.NewArtistMember();
      var member2 = repo.NewArtistMember();
      var member3 = repo.NewArtistMember();


      var artistProfile1 = repo.GetArtistProfile();
      var artistProfile2 = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { member1 });
      var artistProfile3 = repo.GetArtistProfile(genres: new() { genre2, genre3 }, members: new() { member2, member3 });

      context.Genres.AddRange(genre1, genre2, genre3);
      context.ArtistMembers.AddRange(member1, member2, member3);
      context.ArtistProfiles.AddRange(artistProfile1, artistProfile2, artistProfile3);
      context.SaveChanges();


      var artistList = await artistService.GetAllAsync();
      var artist1 = artistList.Find(a => a.Id == artistProfile1.Id);
      var artist2 = artistList.Find(a => a.Id == artistProfile2.Id);
      var artist3 = artistList.Find(a => a.Id == artistProfile3.Id);

      Assert.NotNull(artist1);
      Assert.NotNull(artist2);
      Assert.NotNull(artist3);
      Assert.Equal(3, artistList.Count);
      Assert.Empty(artist1.Members!);
      Assert.Single(artist2.Members!);
      Assert.Equal(2, artist3.Members!.Count);
      Assert.Empty(artist1.Genres!);
      Assert.Equal(2, artist2.Genres!.Count);
      Assert.Equal(2, artist3.Genres!.Count);

    }

    [Fact]
    public async Task CanGetArtistProfileById()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();

      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();

      var member1 = repo.NewArtistMember();
      var member2 = repo.NewArtistMember();

      var artistProfile1 = repo.GetArtistProfile();
      var artistProfile2 = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { member1, member2 });

      context.Genres.AddRange(genre1, genre2);
      context.ArtistMembers.AddRange(member1, member2);
      context.ArtistProfiles.AddRange(artistProfile1, artistProfile2);

      context.SaveChanges();

      var artist1 = await artistService.GetAsync(artistProfile1.Id);
      var artist2 = await artistService.GetAsync(artistProfile2.Id);
      var nullArtist = await artistService.GetAsync(Guid.NewGuid());

      Assert.NotNull(artist1);
      Assert.Equal(artistProfile1.Id, artist1!.Id);
      Assert.NotNull(artist2);
      Assert.Equal(artistProfile2.Id, artist2!.Id);
      Assert.Null(nullArtist);
    }

    [Fact]
    public async Task CanCreateArtistProfile()
    {
      var context = GetInMemoryDbContext();
      var repo = new TestEntityRepository();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);

      var user = repo.NewUser();
      context.Users.Add(user);
      context.SaveChanges();

      ArtistProfileDto dto = new ArtistProfileDto()
      {
        Name = "Test Artist",
        Location = "Test Location",
        Bio = "Test Bio",
        Description = "Test Description",
        Availability = AvailabilityStatus.Closed,
        Genres = new List<GenreDto>()
        {
          new GenreDto("Test Genre 1"),
          new GenreDto("Test Genre 2")
        },
        Members = new List<ArtistMemberDto>()
        {
          new ArtistMemberDto("Test Member 1", "Test Role 1"),
          new ArtistMemberDto("Test Member 2", "Test Role 2")
        }
      };

      var artist = await artistService.CreateAsync(user.Id, dto);

      var retrievedArtist = context.ArtistProfiles.Include(a => a.Genres)
        .Include(a => a.Members)
        .FirstOrDefault(a => a.Id == artist.Id);

      Assert.NotNull(retrievedArtist);
      Assert.Equal(dto.Name, retrievedArtist.Name);
      Assert.Equal(dto.Location, retrievedArtist.Location);
      Assert.Equal(dto.Bio, retrievedArtist.Bio);
      Assert.Equal(dto.Description, retrievedArtist.Description);
      Assert.Equal(dto.Availability, retrievedArtist.Availability);
      Assert.Equal(dto.Genres.Count, retrievedArtist.Genres.Count);
      Assert.Equal(dto.Members.Count, retrievedArtist.Members.Count);
      Assert.Contains(retrievedArtist.Genres, g => g.Name == "Test Genre 1");
      Assert.Contains(retrievedArtist.Genres, g => g.Name == "Test Genre 2");
      Assert.Contains(retrievedArtist.Members, g => g.Name == "Test Member 1");
      Assert.Contains(retrievedArtist.Members, g => g.Name == "Test Member 2");


    }

    [Fact]
    public async Task CanUpdateArtistProfile()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();


      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var artistMember1 = repo.NewArtistMember();
      var artistMember2 = repo.NewArtistMember();
      var artistProfile = repo.GetArtistProfile(genres: new(){genre1, genre2}, members: new() { artistMember1,artistMember2});

      context.ArtistProfiles.Add(artistProfile);
      context.SaveChanges();

      ArtistProfileDto dto = new ArtistProfileDto()
      {
        Id = artistProfile.Id,
        Name = "Updated Artist",
        Location = "Updated Location",
        Bio = "Updated Bio",
        Description = "Updated Description",
        Availability = AvailabilityStatus.Closed,
        Genres = new List<GenreDto>()
        {
          new GenreDto("Updated Genre 1"),
          new GenreDto("Updated Genre 2")
        },
        Members = new List<ArtistMemberDto>()
        {
          new ArtistMemberDto("Updated Member 1", "Updated Role 1"),
          new ArtistMemberDto("Updated Member 2", "Updated Role 2")
        }
      };

      var updatedArtist = await artistService.UpdateAsync(artistProfile.Id, dto);

      var retrievedArtist = context.ArtistProfiles.Include(a => a.Genres)
        .Include(a => a.Members)
        .Include(a => a.Genres)
        .FirstOrDefault(a => a.Id == updatedArtist.Id);

      Assert.NotNull(retrievedArtist);
      Assert.Equal(dto.Name, retrievedArtist.Name);
      Assert.Equal(dto.Location, retrievedArtist.Location);
      Assert.Equal(dto.Bio, retrievedArtist.Bio);
      Assert.Equal(dto.Description, retrievedArtist.Description);
      Assert.Equal(dto.Availability, retrievedArtist.Availability);
      Assert.Equal(dto.Genres.Count, retrievedArtist.Genres.Count);
      Assert.Contains(retrievedArtist.Genres, g => g.Name == "Updated Genre 1");
      Assert.Contains(retrievedArtist.Genres, g => g.Name == "Updated Genre 2");
      Assert.Equal(dto.Members.Count, retrievedArtist.Members.Count);
      Assert.Contains(retrievedArtist.Members, g => g.Name == "Updated Member 1");
      Assert.Contains(retrievedArtist.Members, g => g.Name == "Updated Member 2");
    }

    [Fact]
    public async Task CanDeleteArtistProfile()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();


      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var artistMember1 = repo.NewArtistMember();
      var artistMember2 = repo.NewArtistMember();
      var artistProfile = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { artistMember1, artistMember2 });

      context.ArtistProfiles.Add(artistProfile);
      context.SaveChanges();

      var didDelete = await artistService.DeleteAsync(artistProfile.Id);

      Assert.True(didDelete);
      Assert.Empty(context.ArtistProfiles.ToList());

    }

    [Fact]
    public async Task CanAddArtistMember()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();


      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var artistMember1 = repo.NewArtistMember();
      var artistMember2 = repo.NewArtistMember();
      var artistProfile = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { artistMember1, artistMember2 });

      context.ArtistProfiles.Add(artistProfile);
      context.SaveChanges();

      ArtistMemberDto newMember = new ArtistMemberDto("New Member", "New Role");
      var artistMember = await artistService.AddMember(artistProfile.Id, newMember);

      var retrievedArtist = context.ArtistProfiles.Include(a => a.Members)
        .FirstOrDefault(a => a.Id == artistProfile.Id);

      Assert.NotNull(retrievedArtist);
      Assert.Contains(retrievedArtist.Members, m => m.Name == "New Member");
      Assert.Contains(retrievedArtist.Members, m => m.Role == "New Role");
      Assert.Equal(3, retrievedArtist.Members.Count);
    }

    [Fact]
    public async Task CanUpdateArtistMember()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();


      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var artistMember1 = repo.NewArtistMember();
      var artistMember2 = repo.NewArtistMember();
      var artistProfile = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { artistMember1, artistMember2 });

      context.ArtistProfiles.Add(artistProfile);
      context.SaveChanges();

      ArtistMemberDto updatedMember = new ArtistMemberDto(artistMember1.Id, "Updated Member", "Updated Role");
      ArtistMemberDto fakeMember = new ArtistMemberDto("Fake Updated Member", "Fake Updated Role");
      var artistMember = await artistService.UpdateMember(artistMember1.Id, updatedMember);
      var fakeArtistMember = await artistService.UpdateMember(Guid.NewGuid(), fakeMember);

      var retrievedArtist = context.ArtistProfiles.Include(a => a.Members)
        .FirstOrDefault(a => a.Id == artistProfile.Id);

      Assert.NotNull(retrievedArtist);
      Assert.Contains(retrievedArtist.Members, m => m.Name == "Updated Member");
      Assert.Contains(retrievedArtist.Members, m => m.Role == "Updated Role");
      Assert.DoesNotContain(retrievedArtist.Members, m => m.Name == "Fake Updated Member");
      Assert.DoesNotContain(retrievedArtist.Members, m => m.Role == "Fake Updated Role");
      Assert.Equal(2, retrievedArtist.Members.Count);

    }

    [Fact]
    public async Task CanRemoveArtistMember()
    {
      var context = GetInMemoryDbContext();
      var mediaService = new MediaService(context);
      var genreService = new GenreService(context);
      var artistService = new ArtistService(context, mediaService, genreService);
      var repo = new TestEntityRepository();


      var genre1 = repo.NewGenre();
      var genre2 = repo.NewGenre();
      var artistMember1 = repo.NewArtistMember();
      var artistMember2 = repo.NewArtistMember();
      var artistProfile = repo.GetArtistProfile(genres: new() { genre1, genre2 }, members: new() { artistMember1, artistMember2 });

      context.ArtistProfiles.Add(artistProfile);
      context.SaveChanges();

      var didRemove = await artistService.RemoveMember(artistMember1.Id);

      var retrievedArtist = context.ArtistProfiles.Include(a => a.Members)
        .FirstOrDefault(a => a.Id == artistProfile.Id);

      Assert.NotNull(retrievedArtist);
      Assert.DoesNotContain(retrievedArtist.Members, m => m.Id == artistMember1.Id);

    }


  }
}