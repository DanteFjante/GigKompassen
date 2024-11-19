using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Services;
using GigKompassen.Test.Helpers;

using Microsoft.EntityFrameworkCore;
using static GigKompassen.Test.Helpers.BogusRepositories;
using static GigKompassen.Test.Helpers.DbContextHelper;

namespace GigKompassen.Test.Tests.Services
{
  public class ProfileAccessServiceTests
  {

    private readonly ApplicationDbContext _context;
    private readonly ProfileAccessService _profileAccessService;
    private readonly FakeDataHelper f;

    public ProfileAccessServiceTests() 
    {

      _context = GetInMemoryDbContext();

      _profileAccessService = new ProfileAccessService(_context);

      f = new FakeDataHelper(_context);
    }

    [Fact]
    public async Task CanCheckIfAuthExists()
    {
      var user = f.AddFakeUser()!;
      var user2 = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;
      artist.Public = true;
      var auth = f.AddFakeProfileAccess(user2.Id, artist.Id, AccessType.Edit)!;

      var result1 = await _profileAccessService.CanAccessProfileAsync(user2.Id, artist.Id, AccessType.Edit);
      var result2 = await _profileAccessService.CanAccessProfileAsync(user2.Id, artist.Id, AccessType.View);
      artist.Public = false;
      var result3 = await _profileAccessService.CanAccessProfileAsync(user2.Id, artist.Id, AccessType.View);
      var result4 = await _profileAccessService.CanAccessProfileAsync(user2.Id, artist.Id, AccessType.Represent);

      Assert.True(result1);
      Assert.True(result2);
      Assert.False(result3);
      Assert.False(result3);
    }

    [Fact]
    public async Task CanAddAuth()
    {
      var user = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;

      var auth = await _profileAccessService.AddProfileAuthorizationAsync(user.Id, artist.Id, AccessType.Edit);

      var auths = await _context.ProfileAccesses.Where(pa => pa.ProfileId == artist.Id && pa.UserId == user.Id).ToListAsync();

      Assert.Single(auths);
      Assert.Equal(AccessType.Edit, auths[0].AccessType);
    }

    [Fact]
    public async Task CanRemoveAuth()
    {
      var user = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;
      var scene = f.AddFakeArtistProfile(user.Id)!;
      var auth1 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.Edit)!;
      var auth2 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.View)!;
      var auth3 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.Represent)!;
      var auth4 = f.AddFakeProfileAccess(user.Id, scene.Id, AccessType.Represent)!;

      var result1 = await _profileAccessService.RemoveAuthorizationAsync(user.Id, artist.Id, AccessType.Edit);
      var auth1and2Ids = await _context.ProfileAccesses.Where(pa => pa.ProfileId == artist.Id && pa.UserId == user.Id).Select(pa => pa.Id).ToListAsync();
      var result2 = await _profileAccessService.RemoveAuthorizationAsync(user.Id, scene.Id);
      var result3 = await _profileAccessService.RemoveAuthorizationAsync(auth3.Id);

      var auths = await _context.ProfileAccesses.Where(pa => pa.ProfileId == artist.Id && pa.UserId == user.Id).ToListAsync();

      Assert.True(result1);
      Assert.Equal(2, auth1and2Ids.Count());
      Assert.True(result2);
      Assert.True(result3);
      Assert.Single(auths);
      Assert.Equal(AccessType.View, auths[0].AccessType);
    }

    [Fact]
    public async Task CanClearAuthFromProfile()
    {
      var user = f.AddFakeUser()!;
      var user2 = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;
      var auth1 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.Edit)!;
      var auth2 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.View)!;
      var auth3 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.Represent)!;
      var auth4 = f.AddFakeProfileAccess(user2.Id, artist.Id, AccessType.Represent)!;

      var result = await _profileAccessService.ClearAuthorizationsFromProfileAsync(artist.Id);
      var auths = await _context.ProfileAccesses.ToListAsync();

      Assert.True(result);
      Assert.Empty(auths);
    }

    [Fact]
    public async Task CanClearAuthFromUser()
    {
      var user = f.AddFakeUser()!;
      var user2 = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;
      var artist2 = f.AddFakeArtistProfile(user.Id)!;
      var auth1 = f.AddFakeProfileAccess(user.Id, artist.Id, AccessType.Edit)!;
      var auth2 = f.AddFakeProfileAccess(user2.Id, artist.Id, AccessType.View)!;
      var auth3 = f.AddFakeProfileAccess(user.Id, artist2.Id, AccessType.Represent)!;
      var auth4 = f.AddFakeProfileAccess(user2.Id, artist2.Id, AccessType.Represent)!;

      var result = await _profileAccessService.ClearAuthorizationsFromUserAsync(user.Id);
      var auths = await _context.ProfileAccesses.ToListAsync();


      Assert.True(result);
      Assert.Equal(2, auths.Count());
      Assert.DoesNotContain(auths, a => a.UserId == user.Id);
    } 

    [Fact]
    public async Task CanSetOwner()
    {
      var user = f.AddFakeUser()!;
      var artist = f.AddFakeArtistProfile(user.Id)!;
      var user2 = f.AddFakeUser()!;
      var result = await _profileAccessService.SetProfileOwnerAsync(user2.Id, artist.Id);
      var profile = await _context.ArtistProfiles.Where(ap => ap.Id == artist.Id).FirstOrDefaultAsync();

      Assert.True(result);
      Assert.Equal(user2.Id, profile.OwnerId);
    }
  }
}
