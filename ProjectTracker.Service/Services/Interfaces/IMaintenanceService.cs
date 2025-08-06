using ProjectTracker.Core.Entities;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IMaintenanceService
    {
        Task AddScheduleAsync(int equipmentId, MaintenanceScheduleDto dto);
        Task<IEnumerable<MaintenanceSchedule>> GetUpcomingAsync(int days);
        Task CompleteScheduleAsync(int scheduleId);
    }
}
