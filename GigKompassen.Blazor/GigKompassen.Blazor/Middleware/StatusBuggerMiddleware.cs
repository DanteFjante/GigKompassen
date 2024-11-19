using Microsoft.AspNetCore.Components;

namespace GigKompassen.Blazor.Middleware
{
  public class StatusBuggerMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly NavigationManager _navigationManager;

    public StatusBuggerMiddleware(RequestDelegate next, NavigationManager navigationManager)
    {
      _next = next;
      _navigationManager = navigationManager;
    }

    public async Task InvokeAsync(HttpContext context)
    {
      // Check condition, e.g., if a setup is complete
      var isSetupComplete = context.Session.GetString("IsSetupComplete") == "true"; // Example condition
      var requestPath = context.Request.Path.ToString().ToLower();

      if (!isSetupComplete && !requestPath.Contains("/setup"))
      {
        context.Response.Redirect("/setup");
      }
      else
      {
        await _next(context);
      }
    }
  }
}
