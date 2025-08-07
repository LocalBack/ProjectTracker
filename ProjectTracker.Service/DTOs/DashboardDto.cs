using System.Collections.Generic;

namespace ProjectTracker.Service.DTOs
{
    public class DashboardDto
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        public List<string> UserRoles { get; set; }
        public DashboardStatsDto Stats { get; set; }
        public List<WorkLogDto> RecentWorkLogs { get; set; }
        public List<MaintenanceLogDto> RecentMaintenanceLogs { get; set; }
        public List<EquipmentActionDto> RecentEquipmentActions { get; set; }
        public List<ProjectDto> ActiveProjects { get; set; }
        public List<ProjectReportDto> ProjectReports { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public decimal TotalHoursThisMonth { get; set; }
        public decimal TotalHoursThisWeek { get; set; }
        public int TotalWorkLogs { get; set; }
    }
}