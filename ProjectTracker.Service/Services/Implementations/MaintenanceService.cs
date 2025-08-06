using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly AppDbContext _context;

        public MaintenanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddScheduleAsync(int equipmentId, MaintenanceScheduleDto dto)
        {
            var schedule = new MaintenanceSchedule
            {
                EquipmentId = equipmentId,
                MaintenanceType = dto.MaintenanceType,
                IntervalDays = dto.IntervalDays,
                LastMaintenanceDate = dto.LastMaintenanceDate,
                NextMaintenanceDate = dto.LastMaintenanceDate.AddDays(dto.IntervalDays),
                IsNotificationSent = false
            };
            _context.MaintenanceSchedules.Add(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MaintenanceSchedule>> GetUpcomingAsync(int days)
        {
            var target = DateTime.Today.AddDays(days);
            return await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .Where(m => m.NextMaintenanceDate <= target)
                .ToListAsync();
        }

        public async Task CompleteScheduleAsync(int scheduleId)
        {
            var schedule = await _context.MaintenanceSchedules.FindAsync(scheduleId);
            if (schedule == null) return;

            schedule.LastMaintenanceDate = DateTime.Today;
            schedule.NextMaintenanceDate = DateTime.Today.AddDays(schedule.IntervalDays);
            schedule.IsNotificationSent = false;
            await _context.SaveChangesAsync();
        }
    }
}
