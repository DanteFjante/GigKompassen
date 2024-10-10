using GigKompassen.Models.Accounts;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace GigKompassen.Web.Areas.Identity.Pages.Account
{
  [Authorize]
  public class CompleteProfileModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;

    public CompleteProfileModel(UserManager<ApplicationUser> userManager)
    {
      _userManager = userManager;
    }

    [BindProperty]
    public InputModel Input { get; set; }

    public string AccountType { get; set; }

    public class InputModel
    {
      [Required]
      [Display(Name = "First Name")]
      public string FirstName { get; set; }

      [Required]
      [Display(Name = "Last Name")]
      public string LastName { get; set; }

      // Additional fields based on the user's role
      [Display(Name = "Artist Group Name")]
      public string ArtistGroupName { get; set; }

      [Display(Name = "Scene Name")]
      public string SceneName { get; set; }

      [Display(Name = "Company Name")]
      public string CompanyName { get; set; }
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
        return RedirectToPage("/Index");
      }

      AccountType = await GetUserRoleAsync(user);
      return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
      if (ModelState.IsValid)
      {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
          return RedirectToPage("/Account/Login");
        }

        // Update user properties
        user.FirstName = Input.FirstName;
        user.LastName = Input.LastName;
        user.ProfileCompleted = true;

        // Handle additional information based on role
        var role = await GetUserRoleAsync(user);
        if (role == "Artist")
        {
          // Save Artist Group Profile
          // Implement logic to create and associate an ArtistProfile with the user
        }
        else if (role == "Scene")
        {
          // Save Scene Profile
          // Implement logic to create and associate a SceneProfile with the user
        }
        else if (role == "Manager")
        {
          // Save Manager Profile
          // Implement logic to create and associate a ManagementProfile with the user
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
  }
}
