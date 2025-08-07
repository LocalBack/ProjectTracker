using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Enums;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class UserDashboardController : Controller
    {
        private readonly IUserDashboardService _userDashboardService;
        private readonly IReportingService _reportingService;

        public UserDashboardController(IUserDashboardService userDashboardService, IReportingService reportingService)
        {
            _userDashboardService = userDashboardService;
            _reportingService = reportingService;
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

        [HttpPost]
        public async Task<IActionResult> ExportWorkLogs(string format)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToAction("Login", "Account");

            var exportFormat = Enum.TryParse<ExportFormat>(format, true, out var fmt) ? fmt : ExportFormat.Excel;
            var bytes = await _reportingService.ExportWorkLogsAsync(userId, exportFormat);
            var contentType = fmt == ExportFormat.Excel ?
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            var fileName = fmt == ExportFormat.Excel ? "worklogs.xlsx" : "worklogs.pdf";
            return File(bytes, contentType, fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportActivity(string format)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToAction("Login", "Account");

            var exportFormat = Enum.TryParse<ExportFormat>(format, true, out var fmt) ? fmt : ExportFormat.Excel;
            var bytes = await _reportingService.ExportActivityAsync(userId, exportFormat);
            var contentType = fmt == ExportFormat.Excel ?
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            var fileName = fmt == ExportFormat.Excel ? "activity.xlsx" : "activity.pdf";
            return File(bytes, contentType, fileName);
        }

        [HttpPost]
        public async Task<IActionResult> ExportPerformance(string format)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                return RedirectToAction("Login", "Account");

            var exportFormat = Enum.TryParse<ExportFormat>(format, true, out var fmt) ? fmt : ExportFormat.Excel;
            var bytes = await _reportingService.ExportPerformanceAsync(userId, exportFormat);
            var contentType = fmt == ExportFormat.Excel ?
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" : "application/pdf";
            var fileName = fmt == ExportFormat.Excel ? "performance.xlsx" : "performance.pdf";
            return File(bytes, contentType, fileName);
        }
    }
}