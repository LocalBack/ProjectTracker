using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UserDashboardController : Controller
    {
        private readonly IUserDashboardService _userDashboardService;

        public UserDashboardController(IUserDashboardService userDashboardService)
        {
            _userDashboardService = userDashboardService;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user info
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userName = User.Identity?.Name ?? "Guest";

            // Get dashboard data
            var dashboardData = await _userDashboardService.GetDashboardDataAsync(userId);

            // Set user info
            dashboardData.UserName = userName;
            dashboardData.UserRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return View(dashboardData);
        }
    }
}