using GigKompassen.Authorization.Identity;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Web.Models;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;

namespace GigKompassen.Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IdentityProfileService _identityService;
    private readonly ArtistService _artistService;

    private readonly HomeViewModel model;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, IdentityProfileService identityService, ArtistService artistService)
    {
      _logger = logger;
      _userManager = userManager;
      _identityService = identityService;
      _artistService = artistService;


      model = new HomeViewModel();
    }

    private async Task CreateHomeViewModel()
    {
      model.IsLoggedIn = User.Identity != null && User.Identity.IsAuthenticated;
      model.User = await _userManager.GetUserAsync(User);
    }

    public async Task<IActionResult> Index(MessageViewModel? statusMessage = null)
    {
      await CreateHomeViewModel();
      if (statusMessage != null)
      {
        model.StatusMessage = statusMessage.Message;
      }
      return View(model);
    }

    public async Task<IActionResult> Privacy()
    {
      await CreateHomeViewModel();
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error()
    {
      await CreateHomeViewModel();
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public async Task<IActionResult> DeleteProfile()
    {
      await CreateHomeViewModel();
      ApplicationUser? user = await _userManager.GetUserAsync(User);
      ProfileIdentity? profileIdentity = (await _identityService.GetProfilesFromUserAsync(user)).FirstOrDefault();
      var claims = await _userManager.GetClaimsAsync(user);
      if (user != null)
      {
        user.ProfileCompleted = (await _identityService.GetProfilesFromUserAsync(user)).Count > 0;
        await _userManager.UpdateAsync(user);
      }

      if (!profileIdentity.HasValue)
      {
        model.StatusMessage = "No profile to delete";
        return View("Index", model);
      }
      bool result = await _artistService.DeleteAsync(profileIdentity.Value.ProfileId);
      if(result)
        model.StatusMessage = "Profile Deleted";
      else
        model.StatusMessage = "Profile could not be deleted";



      return View("Index", model);
    }
  }
}
