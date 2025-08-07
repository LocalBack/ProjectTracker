using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IUserDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(int userId, IList<string> roles);
        Task<DashboardStatsDto> GetDashboardStatsAsync(int userId);
        Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int userId, int count = 5);
        Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId);
        Task<IEnumerable<ProjectReportDto>> GetProjectReportsAsync(int userId);
    }
}