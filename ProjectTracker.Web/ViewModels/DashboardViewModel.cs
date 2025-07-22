using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.ViewModels
{
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new();
        public DashboardStatsDto Stats { get; set; } = new();
        public List<WorkLogDto> RecentWorkLogs { get; set; } = new();
        public List<ProjectDto> ActiveProjects { get; set; } = new();

        // Add the missing properties with correct types
        public int TotalProjects { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveWorkLogs { get; set; }  // Changed from List to int
        public int UpcomingMaintenances { get; set; }  // Changed from List to int
    }
}