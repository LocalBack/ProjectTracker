using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<ManageRolesModel> _logger;

        public ManageRolesModel(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<ManageRolesModel> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        [BindProperty]
        public UserRolesViewModel UserRoles { get; set; }

        public class UserRolesViewModel
        {
            public int UserId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FullName { get; set; }
            public List<RoleViewModel> Roles { get; set; } = new List<RoleViewModel>();
        }

        public class RoleViewModel
        {
            public int RoleId { get; set; }
            public string RoleName { get; set; }
            public string Description { get; set; }
            public bool IsAssigned { get; set; }
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

            UserRoles = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            };

            // Get all roles
            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in allRoles)
            {
                UserRoles.Roles.Add(new RoleViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name,
                    Description = GetRoleDescription(role.Name),
                    IsAssigned = userRoles.Contains(role.Name)
                });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByIdAsync(UserRoles.UserId.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Prevent removing Admin role from yourself
            if (user.UserName == User.Identity.Name)
            {
                var adminRoleStillAssigned = UserRoles.Roles.Any(r => r.RoleName == "Admin" && r.IsAssigned);
                if (!adminRoleStillAssigned)
                {
                    TempData["Error"] = "You cannot remove the Admin role from yourself.";
                    return RedirectToPage("./ManageRoles", new { id = UserRoles.UserId });
                }
            }

            // Get current user roles
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Process each role
            foreach (var role in UserRoles.Roles)
            {
                if (role.IsAssigned && !currentRoles.Contains(role.RoleName))
                {
                    // Add role
                    var result = await _userManager.AddToRoleAsync(user, role.RoleName);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Role {Role} added to user {User} by {Admin}",
                            role.RoleName, user.UserName, User.Identity.Name);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else if (!role.IsAssigned && currentRoles.Contains(role.RoleName))
                {
                    // Remove role
                    var result = await _userManager.RemoveFromRoleAsync(user, role.RoleName);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("Role {Role} removed from user {User} by {Admin}",
                            role.RoleName, user.UserName, User.Identity.Name);
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            if (!ModelState.ErrorCount.Equals(0))
            {
                // Reload roles if there were errors
                return await OnGetAsync(UserRoles.UserId);
            }

            TempData["Success"] = $"Roles updated successfully for user {user.UserName}.";
            return RedirectToPage("./Index");
        }

        private string GetRoleDescription(string roleName)
        {
            return roleName switch
            {
                "Admin" => "Full administrative access to the system",
                "Manager" => "Can manage projects and employees",
                "Employee" => "Basic user access with work logging capabilities",
                "ReadOnly" => "View-only access to the system",
                _ => "No description available"
            };
        }
    }
}