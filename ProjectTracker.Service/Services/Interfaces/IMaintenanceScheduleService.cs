using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IMaintenanceScheduleService
    {
        Task<IEnumerable<MaintenanceScheduleDto>> GetAllAsync();
        Task<IEnumerable<MaintenanceScheduleDto>> GetDueAsync();
        Task AddAsync(MaintenanceScheduleDto dto);
        Task MarkNotifiedAsync(int id);
        Task<int> GetUpcomingMaintenanceCountAsync();
    }
}
