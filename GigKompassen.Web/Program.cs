using GigKompassen.Authorization.Policies.Handlers;
using GigKompassen.Authorization.Policies.Requirements;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Services;
using GigKompassen.Settings;
using GigKompassen.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Web
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      var assemblyname = typeof(Program).Assembly.GetName().Name;
      // Add services to the container.
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(connectionString, 
          sqlOptions => sqlOptions.MigrationsAssembly(assemblyname))
          );

      builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
      {
        options.SignIn.RequireConfirmedAccount = true;
      })
      .AddUserManager<UserManager<ApplicationUser>>()
      .AddRoleManager<RoleManager<ApplicationRole>>()
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultUI()
      .AddDefaultTokenProviders();



      builder.Services.AddDatabaseDeveloperPageExceptionFilter();
      AddConfiguration(builder.Configuration, builder.Services);
      ConfigureServices(builder.Services);

      builder.Services.AddAuthorization(options =>
      {
        options.AddPolicy("ProfileComplete", policy => policy.Requirements.Add(new ProfileCompleteRequirement()));
      });

      builder.Services.AddControllersWithViews();

      var app = builder.Build();

      await CreateRoles(app.Services);

      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseMigrationsEndPoint();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      //Custom middleware

      app.UseHttpsRedirection();
      app.UseStaticFiles();

      app.UseRouting();

      app.UseAuthentication();

      
      app.UseAuthorization();

      app.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");
      app.MapRazorPages();

      app.Run();
    }


    public static void AddConfiguration(IConfigurationManager config, IServiceCollection services)
    {
      config.AddUserSecrets<Program>();
      services.Configure<EmailSettings>(config.GetSection(nameof(EmailSettings)));
    }

    public static void ConfigureServices(IServiceCollection services)
    {
      services.AddTransient<IEmailSender, GmailService>();

      services.AddScoped<IAuthorizationHandler, ProfileComleteHandler>();

      services.AddTransient<IdentityProfileService>();
      services.AddTransient<MediaService>();
      services.AddTransient<GenreService>();
      services.AddTransient<ArtistService>();
      services.AddTransient<ManagerProfileService>();
      services.AddTransient<SceneProfileService>();
    
      
    }

    public static async Task CreateRoles(IServiceProvider serviceProvider)
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
