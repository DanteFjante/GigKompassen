using EventKalendern.Core.Services;

using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Settings;
using GigKompassen.Web.Data;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace GigKompassen.Web
{
  public class Program
  {
    public static async void Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(connectionString));

      builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
          options.SignIn.RequireConfirmedAccount = true;
        })
      .AddEntityFrameworkStores<ApplicationDbContext>()
      .AddDefaultUI()
      .AddDefaultTokenProviders();

      builder.Services.AddDatabaseDeveloperPageExceptionFilter();
      AddConfiguration(builder.Configuration, builder.Services);
      ConfigureServices(builder.Services);


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
    }

    public static async Task CreateRoles(IServiceProvider serviceProvider)
    {
      var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

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
