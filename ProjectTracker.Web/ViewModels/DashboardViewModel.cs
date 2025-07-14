using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalProjects { get; set; }
        public int TotalEmployees { get; set; }
        public int ActiveWorkLogs { get; set; }
        public int UpcomingMaintenances { get; set; }
        public IEnumerable<WorkLogDto> RecentWorkLogs { get; set; } = new List<WorkLogDto>();
    }
}