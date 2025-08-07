using ProjectTracker.Core.Entities;
using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IWorkLogService
    {
        Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync();
        Task<WorkLogDto> GetWorkLogByIdAsync(int id);
        Task<WorkLog> GetWorkLogEntityByIdAsync(int id);
        Task<IEnumerable<WorkLogDto>> GetWorkLogsByUserIdAsync(int userId);
        Task<IEnumerable<WorkLogDto>> GetWorkLogsByProjectIdAsync(int projectId);
        Task<IEnumerable<WorkLogDto>> GetWorkLogsByEmployeeIdAsync(int employeeId);  // Add this method
        Task<WorkLogDto> CreateWorkLogAsync(WorkLogDto workLogDto, int userId);
        Task<WorkLogDto> UpdateWorkLogAsync(int id, WorkLogDto workLogDto, int userId);
        Task<bool> DeleteWorkLogAsync(int id, int userId);
        Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int count = 10);
    }
}