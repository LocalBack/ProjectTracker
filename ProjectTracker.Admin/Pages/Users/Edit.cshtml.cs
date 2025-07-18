using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditModel> _logger;

        public EditModel(UserManager<ApplicationUser> userManager, ILogger<EditModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Username { get; set; }

        public class InputModel
        {
            public int Id { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            [StringLength(50)]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            [StringLength(50)]
            public string LastName { get; set; }

            [Display(Name = "Employee ID")]
            [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid employee ID")]
            public int? EmployeeId { get; set; }

            [Phone]
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Email Confirmed")]
            public bool EmailConfirmed { get; set; }

            [Display(Name = "Two-Factor Authentication")]
            public bool TwoFactorEnabled { get; set; }

            [Display(Name = "Account Locked")]
            public bool IsLocked { get; set; }

            [Display(Name = "Lockout End Date")]
            public DateTimeOffset? LockoutEnd { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            Username = user.UserName;

            Input = new InputModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now,
                LockoutEnd = user.LockoutEnd
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(Input.Id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Check if email is being changed and if it's already in use
            if (user.Email != Input.Email)
            {
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError(string.Empty, "Email is already in use by another user.");
                    Username = user.UserName;
                    return Page();
                }
            }

            // Update user properties
            user.Email = Input.Email;
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.EmployeeId = Input.EmployeeId;
            user.PhoneNumber = Input.PhoneNumber;
            user.EmailConfirmed = Input.EmailConfirmed;
            user.TwoFactorEnabled = Input.TwoFactorEnabled;

            // Handle lockout
            if (Input.IsLocked && (!user.LockoutEnd.HasValue || user.LockoutEnd <= DateTimeOffset.Now))
            {
                // Lock the account for 30 days
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.Now.AddDays(30));
                _logger.LogInformation("User {UserName} locked by {Admin}", user.UserName, User.Identity.Name);
            }
            else if (!Input.IsLocked && user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now)
            {
                // Unlock the account
                await _userManager.SetLockoutEndDateAsync(user, null);
                await _userManager.ResetAccessFailedCountAsync(user);
                _logger.LogInformation("User {UserName} unlocked by {Admin}", user.UserName, User.Identity.Name);
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} updated by {Admin}", user.UserName, User.Identity.Name);
                TempData["Success"] = "User has been updated successfully.";
                return RedirectToPage("./Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            Username = user.UserName;
            return Page();
        }

        public async Task<IActionResult> OnPostResetPasswordAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var temporaryPassword = GenerateRandomPassword();

            var result = await _userManager.ResetPasswordAsync(user, token, temporaryPassword);

            if (result.Succeeded)
            {
                // In production, send this via email
                TempData["Success"] = $"Password reset successfully. Temporary password: {temporaryPassword}";
                _logger.LogInformation("Password reset for user {UserName} by {Admin}", user.UserName, User.Identity.Name);
            }
            else
            {
                TempData["Error"] = "Failed to reset password.";
            }

            return RedirectToPage(new { id });
        }

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}