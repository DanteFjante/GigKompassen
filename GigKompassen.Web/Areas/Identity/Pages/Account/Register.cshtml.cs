using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using GigKompassen.Models.Accounts;
using GigKompassen.Web.Models.Identity;
using GigKompassen.Enums;

namespace GigKompassen.Web.Areas.Identity.Pages.Account
{
  public class RegisterModel : PageModel
  {
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IUserEmailStore<ApplicationUser> _emailStore;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        IUserStore<ApplicationUser> userStore,
        SignInManager<ApplicationUser> signInManager,
        ILogger<RegisterModel> logger,
        IEmailSender emailSender)
    {
      _userManager = userManager;
      _emailStore = GetEmailStore(userStore);
      _signInManager = signInManager;
      _logger = logger;
      _emailSender = emailSender;
    }

    [BindProperty]
    public RegistrationViewModel Input { get; set; }

    public string ReturnUrl { get; set; }

    public async Task OnGetAsync(string returnUrl = null)
    {
      ReturnUrl = returnUrl;
    }

    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
      returnUrl ??= Url.Content("~/");
      if (ModelState.IsValid)
      {
        var user = CreateUser();

        // Set UserName and Email
        user.UserName = Input.Username;
        user.Email = Input.Email;

        var result = await _userManager.CreateAsync(user, Input.Password);


        if (result.Succeeded)
        {
          _logger.LogInformation("User created a new account with password.");

          // Assign the role to the user
          var roleName = Input.Role.ConvertToApplicationRoleType().ToString();
          if (!await _userManager.IsInRoleAsync(user, roleName))
          {
            await _userManager.AddToRoleAsync(user, roleName);
          }

          // Generate email confirmation token
          var userId = await _userManager.GetUserIdAsync(user);
          var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
          code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
          var callbackUrl = Url.Page(
              "/Account/ConfirmEmail",
              pageHandler: null,
              values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
              protocol: Request.Scheme);

          await _emailSender.SendEmailAsync(Input.Email, "Finish your registration",
              $"Please finish your registration by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

          if (_userManager.Options.SignIn.RequireConfirmedAccount)
          {
            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
          }
          else
          {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return LocalRedirect(returnUrl);
          }
        }
        AddErrors(result);
      }

      // If we got this far, something failed, redisplay form
      return Page();
    }

    private ApplicationUser CreateUser()
    {
      try
      {
        return new ApplicationUser();
      }
      catch
      {
        throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
            $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor.");
      }
    }

    private IUserEmailStore<ApplicationUser> GetEmailStore(IUserStore<ApplicationUser> userStore)
    {
      if (!_userManager.SupportsUserEmail)
      {
        throw new NotSupportedException("The user manager doesn't support email.");
      }
      return (IUserEmailStore<ApplicationUser>)userStore;
    }

    private void AddErrors(IdentityResult result)
    {
      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }
    }
  }
}