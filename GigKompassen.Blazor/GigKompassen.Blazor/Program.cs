using GigKompassen.Blazor.Client.Pages;
using GigKompassen.Blazor.Components;
using GigKompassen.Blazor.Components.Account;
using GigKompassen.Models.Accounts;
using GigKompassen.Data;
using GigKompassen.Services;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GigKompassen.Misc;
using GigKompassen.Blazor.Components.Account.Shared;
using GigKompassen.Blazor.Models.Status;
using Microsoft.Extensions.Logging;

namespace GigKompassen.Blazor
{
  public class Program
  {
    public static async Task Main(string[] args)
    {
      var builder = WebApplication.CreateBuilder(args);

      // Add services to the container.
      builder.Services.AddRazorComponents()
        .AddInteractiveServerComponents()
        .AddInteractiveWebAssemblyComponents();

      builder.Services.AddCascadingAuthenticationState();
      builder.Services.AddScoped<IdentityUserAccessor>();
      builder.Services.AddScoped<IdentityRedirectManager>();
      builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();

      builder.Services.AddSingleton<StatusCollection>();

      builder.Logging.ClearProviders();
      builder.Logging.AddConsole();
      builder.Logging.AddDebug();

      // Add file logging with Serilog or other providers if needed
      builder.Logging.AddAzureWebAppDiagnostics();

      builder.Services.AddAuthentication(options =>
          {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
          })
          .AddIdentityCookies();

      var assemblyname = typeof(Program).Assembly.GetName().Name;

      var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection") 
        ?? builder.Configuration.GetConnectionString("DefaultConnection") 
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
      builder.Services.AddDbContext<ApplicationDbContext>(options =>
          options.UseSqlServer(
            connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly(assemblyname)));
      builder.Services.AddDatabaseDeveloperPageExceptionFilter();

      builder.AddGigKompassenConfiguration<Program>();
      builder.Services.AddGigKompassenServices();

      builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<ApplicationRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddRoleManager<RoleManager<ApplicationRole>>()
        .AddSignInManager<SignInManager<ApplicationUser>>()
        .AddUserManager<UserManager<ApplicationUser>>()
        .AddDefaultTokenProviders();

      builder.Services.AddSingleton<IEmailSender<ApplicationUser>, GmailService>();

      var app = builder.Build();

      await app.Services.ConfigureGigKompassenRolesAsync();
      // Configure the HTTP request pipeline.
      if (app.Environment.IsDevelopment())
      {
        app.UseWebAssemblyDebugging();
        app.UseMigrationsEndPoint();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }


      app.UseHttpsRedirection();

      app.UseStaticFiles();
      app.UseAntiforgery();

      app.MapRazorComponents<App>()
          .AddInteractiveServerRenderMode()
          .AddInteractiveWebAssemblyRenderMode()
          .AddAdditionalAssemblies(typeof(Client._Imports).Assembly);

      // Add additional endpoints required by the Identity /Account Razor components.
      app.MapAdditionalIdentityEndpoints();

      app.Run();
    }


  }
}
