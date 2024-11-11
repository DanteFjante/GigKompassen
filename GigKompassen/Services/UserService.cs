using GigKompassen.Data;
using GigKompassen.Enums;
using GigKompassen.Models.Accounts;
using GigKompassen.Models.Media;
using GigKompassen.Models.Profiles;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static GigKompassen.Misc.AsyncEventsHelper;


namespace GigKompassen.Services
{
  public class UserService
  {

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;


    public event AsyncEventHandler<ApplicationUser> OnDeleteUser;
    public event AsyncEventHandler<ApplicationUser> OnUserProfileCompleted;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    #region Getters
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid id)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    #endregion

    public async Task<bool> CompleteUserProfileAsync(Guid userId, string firstName, string lastName)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        throw new ArgumentException("First name and last name must be provided");

      if(user.ProfileCompleted)
        return false;

      user.FirstName = firstName;
      user.LastName = lastName;
      user.ProfileCompleted = true;

      if(OnUserProfileCompleted != null)
        await OnUserProfileCompleted.InvokeAsync(this, user);

      var result = await _userManager.UpdateAsync(user);

      return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      if(OnDeleteUser != null)
        await OnDeleteUser.InvokeAsync(this, user);

      var result = await _userManager.DeleteAsync(user);
      return result.Succeeded;
    }
  }
}
