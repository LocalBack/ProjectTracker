using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IWorkLogService
    {
        Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync();
        Task<WorkLogDto?> GetWorkLogByIdAsync(int id); // nullable yapıyoruz
        Task<WorkLogDto> CreateWorkLogAsync(WorkLogDto workLogDto);
        Task<WorkLogDto?> UpdateWorkLogAsync(int id, WorkLogDto workLogDto); // nullable
        Task<bool> DeleteWorkLogAsync(int id);
        Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int count);
        Task<IQueryable<WorkLogDto>> GetAllWorkLogsQueryableAsync();
    }
}