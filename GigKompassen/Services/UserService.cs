﻿using GigKompassen.Data;
using GigKompassen.Models.Accounts;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace GigKompassen.Services
{
  public class UserService
  {

    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
      _context = context;
      _userManager = userManager;
    }

    #region Has
    public async Task<bool> UserHasProfile(Guid userId)
    {
      return await _context.Profiles.AnyAsync(p => p.OwnerId == userId);
    }
    #endregion

    #region Getters
    public async Task<ApplicationUser?> GetUserByIdAsync(Guid id)
    {
      return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    #endregion

    public async Task<bool> CompleteUserRegistrationAsync(Guid userId, string firstName, string lastName)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        throw new ArgumentException("First name and last name must be provided");

      if(user.RegistrationCompleted)
        return false;

      user.FirstName = firstName;
      user.LastName = lastName;
      user.RegistrationCompleted = true;

      var result = await _userManager.UpdateAsync(user);

      return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
      var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
      if (user == null)
        throw new KeyNotFoundException("User not found");

      var result = await _userManager.DeleteAsync(user);
      return result.Succeeded;
    }
  }
}
