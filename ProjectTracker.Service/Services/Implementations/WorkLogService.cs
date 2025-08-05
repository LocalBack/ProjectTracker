using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System;

namespace ProjectTracker.Service.Services.Implementations
{
    public class WorkLogService : IWorkLogService
    {
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<ApplicationUser> _userRepository;
        private readonly IMapper _mapper;

        public WorkLogService(
            IRepository<WorkLog> workLogRepository,
            IRepository<Employee> employeeRepository,
            IRepository<ApplicationUser> userRepository,
            IMapper mapper)
        {
            _workLogRepository = workLogRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<WorkLogDto>> GetAllWorkLogsAsync()
        {
            var workLogs = await _workLogRepository.GetAsync(
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Project,
                    w => w.Employee
                });
            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        public async Task<WorkLogDto> GetWorkLogByIdAsync(int id)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<WorkLog> GetWorkLogEntityByIdAsync(int id)
        {
            var workLogs = await _workLogRepository.GetAsync(
                w => w.Id == id,
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Project,
                    w => w.Project.ProjectEmployees
                });

            return workLogs.FirstOrDefault();
        }

        // Add this method
        public async Task<IEnumerable<WorkLogDto>> GetWorkLogsByUserIdAsync(int userId)
        {
            // Find ApplicationUser by userId
            var users = await _userRepository.GetAsync(u => u.Id == userId);
            var user = users.FirstOrDefault();
            var employeeId = user?.EmployeeId;

            if (employeeId == null)
                return new List<WorkLogDto>();

            // Then get work logs for this employee
            return await GetWorkLogsByEmployeeIdAsync(employeeId.Value);
        }

        // Add this method
        public async Task<IEnumerable<WorkLogDto>> GetWorkLogsByProjectIdAsync(int projectId)
        {
            var workLogs = await _workLogRepository.GetAsync(
                w => w.ProjectId == projectId,
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Employee,
                    w => w.Project
                });

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        // Add this method
        public async Task<IEnumerable<WorkLogDto>> GetWorkLogsByEmployeeIdAsync(int employeeId)
        {
            var workLogs = await _workLogRepository.GetAsync(
                w => w.EmployeeId == employeeId,
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Project,
                    w => w.Employee
                });

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs);
        }

        public async Task<WorkLogDto> CreateWorkLogAsync(WorkLogDto workLogDto)
        {
            var workLog = _mapper.Map<WorkLog>(workLogDto);
            await _workLogRepository.AddAsync(workLog);
            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<WorkLogDto> UpdateWorkLogAsync(int id, WorkLogDto workLogDto)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                return null;

            _mapper.Map(workLogDto, workLog);
            await _workLogRepository.UpdateAsync(workLog);
            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<bool> DeleteWorkLogAsync(int id)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                return false;

            await _workLogRepository.DeleteAsync(workLog);
            return true;
        }
        public async Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int count = 10)
        {
            var workLogs = await _workLogRepository.GetAsync(
                orderBy: q => q.OrderByDescending(w => w.WorkDate),
                includes: new Expression<Func<WorkLog, object>>[]
                {
            w => w.Project,
            w => w.Employee
                });

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs.Take(count));
        }
    }
}