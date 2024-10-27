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
    private readonly ArtistService _artistService;

    private readonly HomeViewModel model;

    public HomeController(ILogger<HomeController> logger, UserManager<ApplicationUser> userManager, ArtistService artistService)
    {
      _logger = logger;
      _userManager = userManager;
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
      var claims = await _userManager.GetClaimsAsync(user);
      if (user != null)
      {
        await _userManager.UpdateAsync(user);
      }




      return View("Index", model);
    }
  }
}
