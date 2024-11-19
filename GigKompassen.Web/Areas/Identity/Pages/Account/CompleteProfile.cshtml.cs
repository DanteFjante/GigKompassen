using GigKompassen.Models.Accounts;
using GigKompassen.Services;
using GigKompassen.Web.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using System.Data;

namespace GigKompassen.Web.Areas.Identity.Pages.Account
{
  [Authorize]
  public class CompleteProfileModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly UserService _userService;
    private readonly ArtistService _artistService;
    private readonly ManagerService _managementService;
    private readonly SceneService _sceneService;

    public CompleteProfileModel(UserManager<ApplicationUser> userManager, UserService userService, ArtistService artistService, ManagerService managementService, SceneService sceneService)
    {
      _userManager = userManager;
      _userService = userService;
      _artistService = artistService;
      _managementService = managementService;
      _sceneService = sceneService;
    }

    [BindProperty]
    public InputModel Input { get; set; }



    public string AccountType { get; set; }
    public bool ProfileCompleted { get; set; }
    [TempData]
    public string ProfileCompleteMessage { get; set; }

    public class InputModel
    {
      [Required]
      [Display(Name = "First Name")]
      public string FirstName { get; set; }

      [Required]
      [Display(Name = "Last Name")]
      public string LastName { get; set; }

    }

    public async Task<IActionResult> OnGetAsync()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return RedirectToPage("/Account/Login");
      }

      
      if (user.RegistrationCompleted)
      {
        // Redirect to home page if profile is already completed
        return RedirectToAction("Index", new MessageViewModel("Redirected to Index Page as profile is already completed"));
      }
      
      AccountType = await GetUserRoleAsync(user);
      ProfileCompleted = user.RegistrationCompleted;
      ProfileCompleteMessage = "Profile already completed";

      /*
      SceneProfile = new CreateSceneProfileViewModel();
      ManagementProfile = new CreateManagementProfileViewModel();
      */
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      var user = await _userManager.GetUserAsync(User);
      if (user == null)
      {
        return RedirectToPage("/Account/Login");
      }
      // Handle additional information based on role
      var role = await GetUserRoleAsync(user);

      List<string> toRemoveKeys = new List<string>();
      int keys = ModelState.Keys.Count(p => p.StartsWith("Artist") || p.StartsWith("Scene") || p.StartsWith("Manager"));
      if (!role.Equals("Artist"))
      {
        toRemoveKeys = toRemoveKeys.Concat(ModelState.Keys.Where(p => p.StartsWith("Artist"))).ToList();
      }
      
      if (!role.Equals("Scene"))
      {
        toRemoveKeys = toRemoveKeys.Concat(ModelState.Keys.Where(p => p.StartsWith("Scene"))).ToList();
      }
      
      if (!role.Equals("Manager"))
      {
        toRemoveKeys = toRemoveKeys.Concat(ModelState.Keys.Where(p => p.StartsWith("Manager"))).ToList();

      }
      foreach (var key in toRemoveKeys)
      {
        ModelState.Remove(key);
      }
      /*
      if (ModelState.All(p => ModelState.GetFieldValidationState(p.Key) != ModelValidationState.Invalid ))
      {
        // Update user properties
        var result = await _userService.CompleteUserProfileAsync(user.Id, Input.FirstName, Input.LastName);

        if (role == "Artist")
        {
          // Save Artist Group Profile
          // Implement logic to create and associate an ArtistProfile with the user
          CreateArtistDto artistProfileDto = new CreateArtistDto(
            ArtistProfile.Name,
            ArtistProfile.Location,
            ArtistProfile.Bio,
            ArtistProfile.Description,
            ArtistProfile.Availability,
            true
            );
          var profile = await _artistService.CreateAsync(user.Id, artistProfileDto, ArtistProfile.Genres, ArtistProfile.Members.Select(m => new ArtistMemberDto(Name: m.Name, Role: m.Role)).ToList());
        }
        else if (role == "Scene")
        {
          // Save Scene Profile
          // Implement logic to create and associate a SceneProfile with the user
          CreateSceneDto sceneProfile = new CreateSceneDto(
            SceneProfile.Name,
            SceneProfile.Address,
            SceneProfile.VenueType,
            SceneProfile.ContactInfo,
            SceneProfile.Capacity,
            SceneProfile.Bio,
            SceneProfile.Description,
            SceneProfile.Amenities,
            SceneProfile.OpeningHours,
            true
            );

          await _sceneService.CreateAsync(user.Id, sceneProfile, SceneProfile.Genres);
        }
        else if (role == "Manager")
        {
          // Save Manager Profile
          // Implement logic to create and associate a ManagerProfile with the user
          CreateManagerDto managerProfile = new CreateManagerDto(
            ManagerProfile.Name,
            ManagerProfile.Description,
            ManagerProfile.Location,
            true
            );

          await _managementService.CreateAsync(user.Id, managerProfile);
        }
        if (result)
        {
          return RedirectToPage("/Index");
        }

        ModelState.AddModelError("UpdateUserError", $"Could not update User: {user.UserName}");

      }
      */

      // If we got this far, something failed; redisplay form
      return Page();
    }

    private async Task<string> GetUserRoleAsync(ApplicationUser user)
    {
      var roles = await _userManager.GetRolesAsync(user);
      return roles.FirstOrDefault();
    }
  }
}
