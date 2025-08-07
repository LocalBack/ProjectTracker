using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IProjectDashboardService
    {
        Task<ProjectSummaryDto> GetSummaryAsync(int projectId);
        Task<IEnumerable<TaskStatusDto>> GetTaskStatusAsync(int projectId);
        Task<IEnumerable<WorkLogTrendDto>> GetWorkLogTrendAsync(int projectId, int months);
        Task<IEnumerable<MaintenanceDto>> GetUpcomingMaintenanceAsync(int projectId, int days);
        Task<IEnumerable<EquipmentStatusDto>> GetEquipmentAsync(int projectId);
        Task<IEnumerable<RecentWorkLogDto>> GetRecentWorkLogsAsync(int projectId, int count);
        Task<IEnumerable<RecentMaintenanceDto>> GetRecentMaintenanceAsync(int projectId, int count);
        Task<byte[]> ExportAsync(ExportTarget target, ExportFormat fmt, int? projectId);
    }
}
