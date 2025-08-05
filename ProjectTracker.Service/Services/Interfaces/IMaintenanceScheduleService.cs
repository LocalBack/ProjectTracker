using ProjectTracker.Service.DTOs;
<<<<<<< HEAD
using System.Collections.Generic;
using System.Threading.Tasks;
=======
>>>>>>> change-tests

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IMaintenanceScheduleService
    {
<<<<<<< HEAD
        Task<IEnumerable<MaintenanceScheduleDto>> GetAllSchedulesAsync();
        Task<MaintenanceScheduleDto> GetScheduleByIdAsync(int id);
        Task<MaintenanceScheduleDto> CreateScheduleAsync(MaintenanceScheduleDto scheduleDto);
        Task<MaintenanceScheduleDto> UpdateScheduleAsync(int id, MaintenanceScheduleDto scheduleDto);
        Task<bool> DeleteScheduleAsync(int id);
        Task<IEnumerable<MaintenanceScheduleDto>> GetUpcomingSchedulesAsync(int daysAhead);
=======
        Task<IEnumerable<MaintenanceScheduleDto>> GetAllAsync();
        Task<IEnumerable<MaintenanceScheduleDto>> GetDueAsync();
        Task AddAsync(MaintenanceScheduleDto dto);
        Task MarkNotifiedAsync(int id);
>>>>>>> change-tests
    }
}
