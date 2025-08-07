using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
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
            dashboardData.ProfileInfo.UserName = userName;
            dashboardData.ProfileInfo.UserRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            var viewModel = new UserDashboardViewModel
            {
                ProfileInfo = new ProfileInfoViewModel
                {
                    UserName = dashboardData.ProfileInfo.UserName,
                    FullName = dashboardData.ProfileInfo.FullName,
                    UserRoles = dashboardData.ProfileInfo.UserRoles
                },
                WorkSummary = new WorkSummaryViewModel
                {
                    ThisWeekHours = dashboardData.WorkSummary.ThisWeekHours,
                    ThisMonthHours = dashboardData.WorkSummary.ThisMonthHours
                },
                Projects = new ProjectsViewModel
                {
                    ActiveProjects = dashboardData.Projects.ActiveProjects
                },
                Activities = new ActivitiesViewModel
                {
                    RecentWorkLogs = dashboardData.Activities.RecentWorkLogs
                },
                Notifications = new NotificationsViewModel
                {
                    Items = dashboardData.Notifications.Items
                },
                Stats = dashboardData.Stats,
                Exports = new ExportsViewModel
                {
                    ProjectReports = dashboardData.Exports.ProjectReports
                }
            };

            return View(viewModel);
        }
    }
}

