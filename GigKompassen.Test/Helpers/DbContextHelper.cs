using GigKompassen.Data;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace GigKompassen.Test.Helpers
{
  internal static class DbContextHelper
  {

    public static ApplicationDbContext GetInMemoryDbContext()
    {
      var options = new DbContextOptionsBuilder<ApplicationDbContext>()
          .UseInMemoryDatabase(Guid.NewGuid().ToString())
          .Options;

      return new ApplicationDbContext(options);
    }
    
    public static UserManager<ApplicationUser> GetMockedUserManager(ApplicationDbContext context)
    {
      var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>(context); // Real store using in-memory context

      var mockUserManager = new UserManager<ApplicationUser>(
          userStore,
          Options.Create(new IdentityOptions()),
          new PasswordHasher<ApplicationUser>(),
          new List<IUserValidator<ApplicationUser>>(),
          new List<IPasswordValidator<ApplicationUser>>(),
          new UpperInvariantLookupNormalizer(),
          new IdentityErrorDescriber(),
          null,
          new Mock<ILogger<UserManager<ApplicationUser>>>().Object
      );

      return mockUserManager;
    }

  }
}
