using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using ProjectTracker.Service.Enums;

namespace ProjectTracker.Web.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectController : Controller
    {
        private readonly IProjectDashboardService _dashboardService;

        public ProjectController(IProjectDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(int projectId)
        {
            var vm = new ProjectDashboardViewModel
            {
                ProjectId = projectId,
                Summary = await _dashboardService.GetSummaryAsync(projectId),
                TaskStatuses = await _dashboardService.GetTaskStatusAsync(projectId),
                WorkLogTrend = await _dashboardService.GetWorkLogTrendAsync(projectId, 12),
                UpcomingMaintenance = await _dashboardService.GetUpcomingMaintenanceAsync(projectId, 30)
            };
            return View(vm);
        }

        public async Task<IActionResult> Export(ExportFormat fmt, ExportTarget target, int? projectId)
        {
            var bytes = await _dashboardService.ExportAsync(target, fmt, projectId);
            var ext = fmt == ExportFormat.Excel ? "xlsx" : "pdf";
            var mime = fmt == ExportFormat.Excel ?
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                "application/pdf";
            return File(bytes, mime, $"export_{System.DateTime.Now:yyyyMMdd}.{ext}");
        }
    }
}
