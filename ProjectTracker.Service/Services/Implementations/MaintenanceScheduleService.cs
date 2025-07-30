using AutoMapper;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Implementations
{
    public class MaintenanceScheduleService : IMaintenanceScheduleService
    {
        private readonly IRepository<MaintenanceSchedule> _scheduleRepository;
        private readonly IMapper _mapper;

        public MaintenanceScheduleService(
            IRepository<MaintenanceSchedule> scheduleRepository,
            IMapper mapper)
        {
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MaintenanceScheduleDto>> GetAllSchedulesAsync()
        {
            var schedules = await _scheduleRepository.GetAsync(
                includes: new Expression<Func<MaintenanceSchedule, object>>[]
                {
                    s => s.Equipment
                });
            return _mapper.Map<IEnumerable<MaintenanceScheduleDto>>(schedules);
        }

        public async Task<MaintenanceScheduleDto> GetScheduleByIdAsync(int id)
        {
            var schedules = await _scheduleRepository.GetAsync(
                s => s.Id == id,
                includes: new Expression<Func<MaintenanceSchedule, object>>[]
                {
                    s => s.Equipment
                });
            var schedule = schedules.FirstOrDefault();
            return _mapper.Map<MaintenanceScheduleDto>(schedule);
        }

        public async Task<MaintenanceScheduleDto> CreateScheduleAsync(MaintenanceScheduleDto scheduleDto)
        {
            var schedule = _mapper.Map<MaintenanceSchedule>(scheduleDto);
            await _scheduleRepository.AddAsync(schedule);
            return _mapper.Map<MaintenanceScheduleDto>(schedule);
        }

        public async Task<MaintenanceScheduleDto> UpdateScheduleAsync(int id, MaintenanceScheduleDto scheduleDto)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                return null;

            _mapper.Map(scheduleDto, schedule);
            await _scheduleRepository.UpdateAsync(schedule);
            return _mapper.Map<MaintenanceScheduleDto>(schedule);
        }

        public async Task<bool> DeleteScheduleAsync(int id)
        {
            var schedule = await _scheduleRepository.GetByIdAsync(id);
            if (schedule == null)
                return false;

            await _scheduleRepository.DeleteAsync(schedule);
            return true;
        }

        public async Task<IEnumerable<MaintenanceScheduleDto>> GetUpcomingSchedulesAsync(int daysAhead)
        {
            DateTime targetDate = DateTime.UtcNow.Date.AddDays(daysAhead);
            var schedules = await _scheduleRepository.GetAsync(
                s => s.NextMaintenanceDate <= targetDate && s.NextMaintenanceDate >= DateTime.UtcNow.Date,
                orderBy: q => q.OrderBy(s => s.NextMaintenanceDate),
                includes: new Expression<Func<MaintenanceSchedule, object>>[]
                {
                    s => s.Equipment
                });
            return _mapper.Map<IEnumerable<MaintenanceScheduleDto>>(schedules);
        }
    }
}
