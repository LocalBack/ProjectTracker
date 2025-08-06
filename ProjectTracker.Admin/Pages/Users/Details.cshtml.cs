using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using ProjectTracker.Core.Entities;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DetailsModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public UserDetailsViewModel UserDetails { get; set; }

        public class UserDetailsViewModel
        {
            public int Id { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int? EmployeeId { get; set; }
            public string PhoneNumber { get; set; }
            public bool EmailConfirmed { get; set; }
            public bool TwoFactorEnabled { get; set; }
            public bool IsLocked { get; set; }
            public DateTimeOffset? LockoutEnd { get; set; }
            public IList<string> Roles { get; set; }
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

            var roles = await _userManager.GetRolesAsync(user);

            UserDetails = new UserDetailsViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmployeeId = user.EmployeeId,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                IsLocked = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.Now,
                LockoutEnd = user.LockoutEnd,
                Roles = roles
            };

            return Page();
        }
    }
}

