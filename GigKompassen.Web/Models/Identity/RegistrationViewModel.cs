using GigKompassen.Enums;

using System.ComponentModel.DataAnnotations;

namespace GigKompassen.Web.Models.Identity
{
  public class RegistrationViewModel
  {

    [Required]
    [Display(Name = "Account Type")]
    public RegisterRoleTypes Role { get; set; }

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Username")]
    public string Username { get; set; }

    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
  }
  public static class RoleConverter
  {
    public static ApplicationRoleTypes ConvertToApplicationRoleType(this RegisterRoleTypes registerRoleType)
    {
      switch (registerRoleType)
      {
        case RegisterRoleTypes.Artist:
          return ApplicationRoleTypes.Artist;
        case RegisterRoleTypes.SceneOwner:
          return ApplicationRoleTypes.SceneOwner;
        case RegisterRoleTypes.Manager:
          return ApplicationRoleTypes.Manager;
        default:
          throw new ArgumentException("Invalid RegisterRoleType value.");
      }
    }
  }

  public enum RegisterRoleTypes
  {
    [Display(Name = "Artist")]
    Artist,
    [Display(Name = "Scene Owner")]
    SceneOwner,
    [Display(Name = "Manager")]
    Manager
  }


}
