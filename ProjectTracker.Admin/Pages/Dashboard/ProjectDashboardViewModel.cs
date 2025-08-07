using ProjectTracker.Service.DTOs;
using System.Collections.Generic;

namespace ProjectTracker.Admin.Pages.Dashboard
{
    public class ProjectDashboardViewModel
    {
        public ProjectSummaryDto Summary { get; set; }
        public IEnumerable<TaskStatusDto> TaskStatuses { get; set; } = System.Linq.Enumerable.Empty<TaskStatusDto>();
        public IEnumerable<WorkLogTrendDto> WorkLogTrend { get; set; } = System.Linq.Enumerable.Empty<WorkLogTrendDto>();
        public IEnumerable<MaintenanceDto> UpcomingMaintenance { get; set; } = System.Linq.Enumerable.Empty<MaintenanceDto>();
        public IEnumerable<EquipmentStatusDto> Equipment { get; set; } = System.Linq.Enumerable.Empty<EquipmentStatusDto>();
        public IEnumerable<RecentWorkLogDto> RecentWorkLogs { get; set; } = System.Linq.Enumerable.Empty<RecentWorkLogDto>();
        public IEnumerable<RecentMaintenanceDto> RecentMaintenance { get; set; } = System.Linq.Enumerable.Empty<RecentMaintenanceDto>();
        public int ProjectId { get; set; }
    }
}
