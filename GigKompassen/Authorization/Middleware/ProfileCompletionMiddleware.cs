using GigKompassen.Models.Accounts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;


namespace GigKompassen.Authorization.Middleware
{
  public class ProfileCompletionMiddleware
  {
    private readonly RequestDelegate _next;

    public ProfileCompletionMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserManager<ApplicationUser> userManager)
    {
      var path = context.Request.Path;

      // List of paths to exclude
      var excludedPaths = new[] {
            "/Account/CompleteProfile",
            "/Account/Logout",
            "/Account/Edit",
            "/Account/Manage",
            "/Identity/Account/Manage",
            "/Error",
            "/favicon.ico",
            "/css/",
            "/js/",
            "/lib/",
            "/images/"
        };

      // Check if the request is for a static file or excluded path
      if (context.User.Identity.IsAuthenticated &&
          path.HasValue &&
          !excludedPaths.Any(p => path.StartsWithSegments(p)) &&
          !path.Value.EndsWith(".css") &&
          !path.Value.EndsWith(".js") &&
          !path.Value.EndsWith(".png") &&
          !path.Value.EndsWith(".jpg") &&
          !path.Value.EndsWith(".jpeg") &&
          !path.Value.EndsWith(".gif"))
      {
        var user = await userManager.GetUserAsync(context.User);
        if (user != null && !user.RegistrationCompleted)
        {
          context.Response.Redirect("/Account/CompleteProfile");
          return;
        }
      }

      await _next(context);
    }
  }
}
