using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Enums;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Implementations
{
    public class ProjectDashboardService : IProjectDashboardService
    {
        public Task<ProjectSummaryDto> GetSummaryAsync(int projectId)
        {
            var dto = new ProjectSummaryDto();
            return Task.FromResult(dto);
        }

        public Task<IEnumerable<TaskStatusDto>> GetTaskStatusAsync(int projectId)
        {
            IEnumerable<TaskStatusDto> list = new List<TaskStatusDto>();
            return Task.FromResult(list);
        }

        public Task<IEnumerable<WorkLogTrendDto>> GetWorkLogTrendAsync(int projectId, int months)
        {
            IEnumerable<WorkLogTrendDto> list = new List<WorkLogTrendDto>();
            return Task.FromResult(list);
        }

        public Task<IEnumerable<MaintenanceDto>> GetUpcomingMaintenanceAsync(int projectId, int days)
        {
            IEnumerable<MaintenanceDto> list = new List<MaintenanceDto>();
            return Task.FromResult(list);
        }

        public Task<byte[]> ExportAsync(ExportTarget target, ExportFormat fmt, int? projectId)
        {
            byte[] bytes = System.Array.Empty<byte>();
            return Task.FromResult(bytes);
        }
    }
}
