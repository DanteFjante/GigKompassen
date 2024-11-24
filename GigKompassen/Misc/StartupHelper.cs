using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Services;
using GigKompassen.Settings;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GigKompassen.Misc
{
  public static class StartupHelper
  {

    public static void AddGigKompassenConfiguration<AssemblyType>(this IHostApplicationBuilder builder) where AssemblyType : class
    {
      builder.Configuration.AddUserSecrets<AssemblyType>();
      builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSender"));
    }

    public static void AddGigKompassenServices(this IServiceCollection services)
    {
      services.AddTransient<IEmailSender<ApplicationUser>, GmailService>();

      services.AddTransient<ProfileService>();
      services.AddTransient<ArtistService>();
      services.AddTransient<ManagerService>();
      services.AddTransient<SceneService>();
      services.AddTransient<MediaService>();
      services.AddTransient<ProfileAccessService>();
      services.AddTransient<UserService>();
      services.AddTransient<GenreService>();
    }

    public static async Task ConfigureGigKompassenRolesAsync(this IServiceProvider serviceProvider)
    {
      using (var scope = serviceProvider.CreateScope())
      {

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        foreach (var roleName in Enum.GetNames(typeof(ApplicationRoleTypes)))
        {
          if (!await roleManager.RoleExistsAsync(roleName))
          {
            await roleManager.CreateAsync(new ApplicationRole(roleName));
          }
        }

      }
    }
  }
}
