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
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkLogHistory> _historyRepository;
        private readonly IMapper _mapper;

        public WorkLogService(
            IRepository<WorkLog> workLogRepository,
            IRepository<Employee> employeeRepository,
            IRepository<ApplicationUser> userRepository,
            IRepository<Project> projectRepository,
            IRepository<WorkLogHistory> historyRepository,
            IMapper mapper)
        {
            _workLogRepository = workLogRepository;
            _employeeRepository = employeeRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _historyRepository = historyRepository;
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
            var workLogs = await _workLogRepository.GetAsync(
                w => w.Id == id,
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Project,
                    w => w.Employee,
                    w => w.Details,
                    w => w.Attachments
                });

            var workLog = workLogs.FirstOrDefault();
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
            // Attempt to resolve the employee id from the user record or by reverse lookup
            var users = await _userRepository.GetAsync(u => u.Id == userId, null, u => u.Employee);
            var user = users.FirstOrDefault();

            int? employeeId = user?.EmployeeId ?? user?.Employee?.Id;

            if (employeeId == null)
            {
                var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
                var employee = employees.FirstOrDefault();
                employeeId = employee?.Id;
            }

            if (employeeId == null)
            {
                return new List<WorkLogDto>();
            }

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

        public async Task<WorkLogDto> CreateWorkLogAsync(WorkLogDto workLogDto, int userId)
        {
            var workLog = _mapper.Map<WorkLog>(workLogDto);
            await _workLogRepository.AddAsync(workLog);

            if (workLog.Cost > 0)
            {
                var project = await _projectRepository.GetByIdAsync(workLog.ProjectId);
                if (project != null)
                {
                    project.ActualCost = (project.ActualCost ?? 0) + workLog.Cost;
                    await _projectRepository.UpdateAsync(project);
                }
            }

            var user = await _userRepository.GetByIdAsync(userId);
            var history = new WorkLogHistory
            {
                WorkLogId = workLog.Id,
                ChangedByUserId = userId,
                ChangedByUserName = user?.UserName ?? string.Empty,
                Action = "Created",
                Changes = $"Title: {workLog.Title}, Description: {workLog.Description}, WorkDate: {workLog.WorkDate:d}, HoursSpent: {workLog.HoursSpent}, Cost: {workLog.Cost}, ProjectId: {workLog.ProjectId}, EmployeeId: {workLog.EmployeeId}"
            };
            await _historyRepository.AddAsync(history);

            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<WorkLogDto> UpdateWorkLogAsync(int id, WorkLogDto workLogDto, int userId)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                return null;
            var oldCost = workLog.Cost;
            var original = new WorkLog
            {
                Title = workLog.Title,
                Description = workLog.Description,
                WorkDate = workLog.WorkDate,
                HoursSpent = workLog.HoursSpent,
                Cost = workLog.Cost,
                ProjectId = workLog.ProjectId,
                EmployeeId = workLog.EmployeeId
            };

            _mapper.Map(workLogDto, workLog);
            await _workLogRepository.UpdateAsync(workLog);

            if (oldCost != workLog.Cost)
            {
                var project = await _projectRepository.GetByIdAsync(workLog.ProjectId);
                if (project != null)
                {
                    project.ActualCost = (project.ActualCost ?? 0) - oldCost + workLog.Cost;
                    await _projectRepository.UpdateAsync(project);
                }
            }

            var changes = new List<string>();
            if (original.Title != workLog.Title) changes.Add($"Title: '{original.Title}' -> '{workLog.Title}'");
            if (original.Description != workLog.Description) changes.Add($"Description changed");
            if (original.WorkDate != workLog.WorkDate) changes.Add($"WorkDate: {original.WorkDate:d} -> {workLog.WorkDate:d}");
            if (original.HoursSpent != workLog.HoursSpent) changes.Add($"HoursSpent: {original.HoursSpent} -> {workLog.HoursSpent}");
            if (original.Cost != workLog.Cost) changes.Add($"Cost: {original.Cost} -> {workLog.Cost}");
            if (original.ProjectId != workLog.ProjectId) changes.Add($"ProjectId: {original.ProjectId} -> {workLog.ProjectId}");
            if (original.EmployeeId != workLog.EmployeeId) changes.Add($"EmployeeId: {original.EmployeeId} -> {workLog.EmployeeId}");

            var user = await _userRepository.GetByIdAsync(userId);
            var history = new WorkLogHistory
            {
                WorkLogId = workLog.Id,
                ChangedByUserId = userId,
                ChangedByUserName = user?.UserName ?? string.Empty,
                Action = "Updated",
                Changes = string.Join("; ", changes)
            };
            await _historyRepository.AddAsync(history);

            return _mapper.Map<WorkLogDto>(workLog);
        }

        public async Task<bool> DeleteWorkLogAsync(int id, int userId)
        {
            var workLog = await _workLogRepository.GetByIdAsync(id);
            if (workLog == null)
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            var history = new WorkLogHistory
            {
                WorkLogId = workLog.Id,
                ChangedByUserId = userId,
                ChangedByUserName = user?.UserName ?? string.Empty,
                Action = "Deleted",
                Changes = $"Title: {workLog.Title}, Description: {workLog.Description}, WorkDate: {workLog.WorkDate:d}, HoursSpent: {workLog.HoursSpent}, Cost: {workLog.Cost}, ProjectId: {workLog.ProjectId}, EmployeeId: {workLog.EmployeeId}"
            };
            await _historyRepository.AddAsync(history);

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