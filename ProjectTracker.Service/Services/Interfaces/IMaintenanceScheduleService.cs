using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IMaintenanceScheduleService
    {
        Task<IEnumerable<MaintenanceScheduleDto>> GetAllSchedulesAsync();
        Task<MaintenanceScheduleDto> GetScheduleByIdAsync(int id);
        Task<MaintenanceScheduleDto> CreateScheduleAsync(MaintenanceScheduleDto scheduleDto);
        Task<MaintenanceScheduleDto> UpdateScheduleAsync(int id, MaintenanceScheduleDto scheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<IEnumerable<MaintenanceScheduleDto>> GetUpcomingSchedulesAsync(int daysAhead);
    }
}
