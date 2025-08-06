using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class MaintenanceScheduleService : IMaintenanceScheduleService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public MaintenanceScheduleService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaintenanceScheduleDto>> GetAllAsync()
        {
            var items = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .Include(m => m.Project)
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaintenanceScheduleDto>>(items);
        }

        public async Task<IEnumerable<MaintenanceScheduleDto>> GetDueAsync()
        {
            var now = DateTime.Today;
            var items = await _context.MaintenanceSchedules
                .Where(m => m.NextMaintenanceDate <= now && !m.IsNotificationSent)
                .Include(m => m.Equipment)
                .Include(m => m.Project)
                .ToListAsync();
            return _mapper.Map<IEnumerable<MaintenanceScheduleDto>>(items);
        }

        public async Task AddAsync(MaintenanceScheduleDto dto)
        {
            var entity = _mapper.Map<MaintenanceSchedule>(dto);
            entity.NextMaintenanceDate = dto.LastMaintenanceDate.AddDays(dto.IntervalDays);
            _context.MaintenanceSchedules.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task MarkNotifiedAsync(int id)
        {
            var entity = await _context.MaintenanceSchedules.FindAsync(id);
            if (entity != null)
            {
                entity.IsNotificationSent = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
