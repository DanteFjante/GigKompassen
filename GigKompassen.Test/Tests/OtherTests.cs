using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;

using Microsoft.EntityFrameworkCore;

using static GigKompassen.Test.Helpers.DbContextHelper;
using static GigKompassen.Test.Helpers.BogusRepositories;

namespace GigKompassen.Test.Tests
{
  public class OtherTests
  {

    public OtherTests() { }

    [Fact]
    public async Task Should_Create_User()
    {
      var context = GetInMemoryDbContext();
      // Arrange
      var userManager = GetMockedUserManager(context);
      var user = UserFaker.Generate();
      // Act
      var result = await userManager.CreateAsync(user, "password");
      // Assert
      var retrievedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
      Assert.True(result.Succeeded);
      Assert.NotNull(retrievedUser);
    }

    [Fact]
    public async Task Should_Find_User_By_Email()
    {
      var context = GetInMemoryDbContext();
      // Arrange
      var userManager = GetMockedUserManager(context);
      var email = "testuser@example.com";
      var username = "testuser";
      var user = UserFaker.Generate();
      user.Email = email;
      user.UserName = username;
      await userManager.CreateAsync(user, "password");

      // Act
      var retrievedUser = await userManager.FindByEmailAsync(email);

      // Assert
      Assert.NotNull(retrievedUser);
      Assert.Equal("testuser", retrievedUser.UserName);
    }
  }
}
