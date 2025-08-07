using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<ProjectEmployee> _projectEmployeeRepository;
        private readonly IRepository<MaintenanceLog> _maintenanceLogRepository;
        private readonly IMapper _mapper;

        public UserDashboardService(
            IRepository<Employee> employeeRepository,
            IRepository<WorkLog> workLogRepository,
            IRepository<Project> projectRepository,
            IRepository<ProjectEmployee> projectEmployeeRepository,
            IRepository<MaintenanceLog> maintenanceLogRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _workLogRepository = workLogRepository;
            _projectRepository = projectRepository;
            _projectEmployeeRepository = projectEmployeeRepository;
            _maintenanceLogRepository = maintenanceLogRepository;
            _mapper = mapper;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(int userId)
        {
            // Initialize with default values to prevent null reference exceptions
            var dashboard = new DashboardDto
            {
                UserName = string.Empty, // You need to set this from somewhere
                FullName = "Guest User",
                UserRoles = new List<string>(),
                Stats = new DashboardStatsDto // Initialize Stats to prevent null
                {
                    TotalProjects = 0,
                    ActiveProjects = 0,
                    CompletedProjects = 0,
                    TotalHoursThisMonth = 0,
                    TotalHoursThisWeek = 0,
                    TotalWorkLogs = 0
                },
                RecentWorkLogs = new List<WorkLogDto>(),
                RecentMaintenanceLogs = new List<MaintenanceLogDto>(),
                RecentEquipmentActions = new List<EquipmentActionDto>(),
                ActiveProjects = new List<ProjectDto>(),
                ProjectReports = new List<ProjectReportDto>()
            };

            // Get employee data
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee != null)
            {
                dashboard.FullName = $"{employee.FirstName} {employee.LastName}";

                // Get stats - this will overwrite the default values
                dashboard.Stats = await GetDashboardStatsAsync(userId);

                // Get recent work logs
                dashboard.RecentWorkLogs = (await GetRecentWorkLogsAsync(userId, 5)).ToList();

                // Get recent maintenance logs
                dashboard.RecentMaintenanceLogs = (await GetRecentMaintenanceLogsAsync(userId, 5)).ToList();

                // Get recent equipment actions
                dashboard.RecentEquipmentActions = (await GetRecentEquipmentActionsAsync(userId, 5)).ToList();

                // Get active projects
                dashboard.ActiveProjects = (await GetUserProjectsAsync(userId)).ToList();

                // Get project reports
                dashboard.ProjectReports = (await GetProjectReportsAsync(userId)).ToList();
            }

            return dashboard;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
        {
            var stats = new DashboardStatsDto();

            // Get employee
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee != null)
            {
                // Get employee's projects
                var projectEmployees = await _projectEmployeeRepository.GetAsync(
                    pe => pe.EmployeeId == employee.Id,
                    includes: new Expression<Func<ProjectEmployee, object>>[]
                    {
                        pe => pe.Project
                    });

                stats.TotalProjects = projectEmployees.Count();
                stats.ActiveProjects = projectEmployees.Count(pe => pe.Project.Status == ProjectStatus.Active);
                stats.CompletedProjects = projectEmployees.Count(pe => pe.Project.Status == ProjectStatus.Completed);

                // Get work logs stats
                var workLogs = await _workLogRepository.GetAsync(w => w.EmployeeId == employee.Id);
                stats.TotalWorkLogs = workLogs.Count();

                // This month's hours
                var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                stats.TotalHoursThisMonth = workLogs
                    .Where(w => w.WorkDate >= startOfMonth)
                    .Sum(w => w.HoursSpent);

                // This week's hours
                var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                stats.TotalHoursThisWeek = workLogs
                    .Where(w => w.WorkDate >= startOfWeek)
                    .Sum(w => w.HoursSpent);
            }

            return stats;
        }

        public async Task<IEnumerable<WorkLogDto>> GetRecentWorkLogsAsync(int userId, int count = 5)
        {
            // Get employee
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                return new List<WorkLogDto>();

            var workLogs = await _workLogRepository.GetAsync(
                w => w.EmployeeId == employee.Id,
                orderBy: q => q.OrderByDescending(w => w.WorkDate),
                includes: new Expression<Func<WorkLog, object>>[]
                {
                    w => w.Project
                });

            return _mapper.Map<IEnumerable<WorkLogDto>>(workLogs.Take(count));
        }

        public async Task<IEnumerable<MaintenanceLogDto>> GetRecentMaintenanceLogsAsync(int userId, int count = 5)
        {
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                return new List<MaintenanceLogDto>();

            var logs = await _maintenanceLogRepository.GetAsync(
                l => l.MaintenanceSchedule.Project.ProjectEmployees.Any(pe => pe.EmployeeId == employee.Id),
                orderBy: q => q.OrderByDescending(l => l.MaintenanceDate),
                includes: new Expression<Func<MaintenanceLog, object>>[]
                {
                    l => l.MaintenanceSchedule,
                    l => l.MaintenanceSchedule.Equipment,
                    l => l.MaintenanceSchedule.Project
                });

            return logs.Take(count).Select(l => new MaintenanceLogDto
            {
                Date = l.MaintenanceDate,
                Equipment = l.MaintenanceSchedule.Equipment.Name,
                Description = l.Notes,
                Status = l.IsCompleted ? "Completed" : "Pending"
            });
        }

        public async Task<IEnumerable<EquipmentActionDto>> GetRecentEquipmentActionsAsync(int userId, int count = 5)
        {
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                return new List<EquipmentActionDto>();

            var actions = await _maintenanceLogRepository.GetAsync(
                l => l.MaintenanceSchedule.Project.ProjectEmployees.Any(pe => pe.EmployeeId == employee.Id),
                orderBy: q => q.OrderByDescending(l => l.MaintenanceDate),
                includes: new Expression<Func<MaintenanceLog, object>>[]
                {
                    l => l.MaintenanceSchedule,
                    l => l.MaintenanceSchedule.Equipment,
                    l => l.MaintenanceSchedule.Project
                });

            return actions.Take(count).Select(l => new EquipmentActionDto
            {
                Date = l.MaintenanceDate,
                Equipment = l.MaintenanceSchedule.Equipment.Name,
                Description = l.Notes,
                Status = l.IsCompleted ? "Completed" : "Pending"
            });
        }

        public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(int userId)
        {
            // Get employee
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();

            if (employee == null)
                return new List<ProjectDto>();

            var projectEmployees = await _projectEmployeeRepository.GetAsync(
                pe => pe.EmployeeId == employee.Id && pe.Project.Status == ProjectStatus.Active,
                includes: new Expression<Func<ProjectEmployee, object>>[]
                {
                    pe => pe.Project
                });

            var projects = projectEmployees.Select(pe => pe.Project).Distinct();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<IEnumerable<ProjectReportDto>> GetProjectReportsAsync(int userId)
        {
            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();
            if (employee == null)
                return new List<ProjectReportDto>();

            var projects = await _projectRepository.GetAsync(
                p => p.ProjectEmployees.Any(pe => pe.EmployeeId == employee.Id),
                includes: new Expression<Func<Project, object>>[]
                {
                    p => p.WorkLogs,
                    p => p.ProjectEmployees
                });

            var reports = projects.Select(p => new ProjectReportDto
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                TotalHours = p.WorkLogs.Sum(w => w.HoursSpent),
                TotalCost = p.WorkLogs.Sum(w => w.Cost),
                Budget = p.Budget,
                ActualCost = p.ActualCost ?? p.WorkLogs.Sum(w => w.Cost)
            });

            return reports;
        }
    }
}