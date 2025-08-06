using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(UserManager<ApplicationUser> userManager, ILogger<IndexModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<UserViewModel> Users { get; set; }

        public class UserViewModel
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string EmployeeId { get; set; }
            public bool IsAdmin { get; set; }
            public IList<string> Roles { get; set; }
        }

        public async Task OnGetAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            Users = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Users.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmployeeId = user.EmployeeId?.ToString() ?? string.Empty,
                    IsAdmin = roles.Contains("Admin"),
                    Roles = roles
                });
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Prevent deleting yourself
            if (user.UserName == User.Identity.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToPage();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User {UserName} deleted by {AdminUser}", user.UserName, User.Identity.Name);
                TempData["Success"] = $"User {user.UserName} has been deleted.";
            }
            else
            {
                TempData["Error"] = $"Error deleting user: {string.Join(", ", result.Errors.Select(e => e.Description))}";
            }

            return RedirectToPage();
        }
    }
}