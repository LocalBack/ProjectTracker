using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.Enums;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Dashboard
{
    [Authorize(Roles = "Admin,Manager")]
    public class ProjectModel : PageModel
    {
        private readonly IProjectDashboardService _dashboardService;

        public ProjectDashboardViewModel Dashboard { get; set; } = new();
        public int ProjectId { get; set; }

        public ProjectModel(IProjectDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task OnGetAsync(int projectId)
        {
            ProjectId = projectId;
            Dashboard = new ProjectDashboardViewModel
            {
                ProjectId = projectId,
                Summary = await _dashboardService.GetSummaryAsync(projectId),
                TaskStatuses = await _dashboardService.GetTaskStatusAsync(projectId),
                WorkLogTrend = await _dashboardService.GetWorkLogTrendAsync(projectId, 12),
                UpcomingMaintenance = await _dashboardService.GetUpcomingMaintenanceAsync(projectId, 30),
                Equipment = await _dashboardService.GetEquipmentAsync(projectId),
                RecentWorkLogs = await _dashboardService.GetRecentWorkLogsAsync(projectId, 5),
                RecentMaintenance = await _dashboardService.GetRecentMaintenanceAsync(projectId, 5)
            };
        }

        public async Task<IActionResult> OnGetExportAsync(string fmt, int projectId)
        {
            var format = fmt.Equals("pdf", StringComparison.OrdinalIgnoreCase) ? ExportFormat.Pdf : ExportFormat.Excel;
            var bytes = await _dashboardService.ExportAsync(ExportTarget.WorkLogs, format, projectId);
            var ext = format == ExportFormat.Excel ? "xlsx" : "pdf";
            var mime = format == ExportFormat.Excel ?
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" :
                "application/pdf";
            return File(bytes, mime, $"export_{System.DateTime.Now:yyyyMMdd}.{ext}");
        }
    }
}
