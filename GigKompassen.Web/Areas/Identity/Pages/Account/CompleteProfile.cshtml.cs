using GigKompassen.Authorization.Identity;
using GigKompassen.Dto.Profiles;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;
using GigKompassen.Services;
using GigKompassen.Web.Models;
using GigKompassen.Web.Models.Profiles.Create;

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
    private readonly ArtistService _artistService;
    private readonly ManagerProfileService _managementService;
    private readonly SceneProfileService _sceneService;
    private readonly IdentityProfileService _identityService;

    public CompleteProfileModel(UserManager<ApplicationUser> userManager, ArtistService artistService, ManagerProfileService managementService, SceneProfileService sceneService, IdentityProfileService identityService)
    {
      _userManager = userManager;
      _artistService = artistService;
      _managementService = managementService;
      _sceneService = sceneService;
      _identityService = identityService;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    [BindProperty(Name = "ArtistProfile")]
    public CreateArtistProfileViewModel ArtistProfile { get; set; }

    [BindProperty(Name = "SceneProfile")]
    public CreateSceneProfileViewModel SceneProfile { get; set; }

    [BindProperty(Name = "ManagerProfile")]
    public CreateManagementProfileViewModel ManagerProfile { get; set; }

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

      
      if (user.ProfileCompleted)
      {
        // Redirect to home page if profile is already completed
        return RedirectToAction("Index", new MessageViewModel("Redirected to Index Page as profile is already completed"));
      }
      
      AccountType = await GetUserRoleAsync(user);
      ProfileCompleted = user.ProfileCompleted;
      ProfileCompleteMessage = "Profile already completed";

      ArtistProfile = new CreateArtistProfileViewModel();
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

      if (ModelState.All(p => ModelState.GetFieldValidationState(p.Key) != ModelValidationState.Invalid ))
      {
        // Update user properties
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.ProfileCompleted = true;

        if (role == "Artist")
        {
          // Save Artist Group Profile
          // Implement logic to create and associate an ArtistProfile with the user
          ArtistProfileDto artistProfile = FromViewModel(ArtistProfile);
          var profile = await _artistService.CreateAsync(artistProfile);
          await _identityService.AddUserToProfileAsync(user, new ProfileIdentity(profile.Id, ProfileTypeEnum.Artist));
        }
        else if (role == "Scene")
        {
          // Save Scene Profile
          // Implement logic to create and associate a SceneProfile with the user
          SceneProfileDto sceneProfile = FromViewModel(SceneProfile);

          await _sceneService.CreateAsync(sceneProfile);
        }
        else if (role == "Manager")
        {
          // Save Manager Profile
          // Implement logic to create and associate a ManagementProfile with the user
          ManagerProfileDto managementProfile = FromViewModel(ManagerProfile);

          await _managementService.CreateAsync(managementProfile);
        }

        var result = await _userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
          return RedirectToPage("/Index");
        }

        foreach (var error in result.Errors)
        {
          ModelState.AddModelError(string.Empty, error.Description);
        }
      }

      // If we got this far, something failed; redisplay form
      return Page();
    }

    private async Task<string> GetUserRoleAsync(ApplicationUser user)
    {
      var roles = await _userManager.GetRolesAsync(user);
      return roles.FirstOrDefault();
    }
    
    private ArtistProfileDto FromViewModel(CreateArtistProfileViewModel viewModel)
    {
      ArtistProfileDto profile = new()
      {
        Name = viewModel.Name,
        Location = viewModel.Location,
        Bio = viewModel.Bio,
        Description = viewModel.Description,
        Availability = viewModel.Availability,
        Genres = viewModel.Genres.Select(p => new GenreDto(p)).ToList(),
        Members = viewModel.Members.Select(p => new ArtistMemberDto(p.Name, p.Role)).ToList()
      };
      return profile;
    }



    private SceneProfileDto FromViewModel(CreateSceneProfileViewModel viewModel)
    {
      SceneProfileDto profile = new()
      {
        VenueName = viewModel.VenueName,
        Address = viewModel.Address,
        VenueType = viewModel.VenueType,
        ContactInfo = viewModel.ContactInfo,
        Capacity = viewModel.Capacity,
        Amenities = viewModel.Amenities,
        Bio = viewModel.Bio,
        Description = viewModel.Description,
        OpeningHours = viewModel.OpeningHours,
        Genres = viewModel.Genres.Select(p => new GenreDto(p)).ToList()
      };
      return profile;
    }

    private ManagerProfileDto FromViewModel(CreateManagementProfileViewModel viewModel)
    {
      ManagerProfileDto profile = new()
      {
        Name = viewModel.Name,
        Description = viewModel.Description,
        Location = viewModel.Location
      };
      return profile;
    }
  }
}
