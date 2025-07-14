using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IWorkLogService
    {
        Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync();
        Task<IEnumerable<WorkLogDto>> GetWorkLogsByProjectIdAsync(int projectId);
        Task<IEnumerable<WorkLogDto>> GetWorkLogsByEmployeeIdAsync(int employeeId);
        Task<WorkLogDto?> GetWorkLogByIdAsync(int id);
        Task<IEnumerable<WorkLogDetailDto>> GetWorkLogDetailsAsync(int workLogId);
        Task<IEnumerable<WorkLogAttachmentDto>> GetWorkLogAttachmentsAsync(int workLogId);
        Task<WorkLogDto> CreateWorkLogAsync(CreateWorkLogDto createWorkLogDto);
        Task UpdateWorkLogAsync(int id, WorkLogDto workLogDto);
        Task DeleteWorkLogAsync(int id);

        Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int count);
    }
}