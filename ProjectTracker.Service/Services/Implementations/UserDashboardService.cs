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

        private readonly IRepository<Equipment> _equipmentRepository;

        private readonly IMapper _mapper;

        public UserDashboardService(
            IRepository<Employee> employeeRepository,
            IRepository<WorkLog> workLogRepository,
            IRepository<Project> projectRepository,
            IRepository<ProjectEmployee> projectEmployeeRepository,
            IRepository<MaintenanceLog> maintenanceLogRepository,

            IRepository<Equipment> equipmentRepository,

            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _workLogRepository = workLogRepository;
            _projectRepository = projectRepository;
            _projectEmployeeRepository = projectEmployeeRepository;
            _maintenanceLogRepository = maintenanceLogRepository;

            _equipmentRepository = equipmentRepository;

            _mapper = mapper;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(int userId, IList<string> roles)
        {
            // Initialize with default values to prevent null reference exceptions
            var dashboard = new DashboardDto
            {
                UserName = string.Empty,
                FullName = "Guest User",
                UserRoles = new List<string>(),
                Stats = new DashboardStatsDto
                {
                    TotalProjects = 0,
                    ActiveProjects = 0,
                    CompletedProjects = 0,
                    PendingWorkLogApprovals = 0,
                    ActiveTasks = 0,
                    CompletedTasks = 0,
                    TotalEquipment = 0,
                    TotalHoursThisMonth = 0,
                    TotalHoursThisWeek = 0,
                    TotalWorkLogs = 0,
                    WeeklyHours = new List<decimal>()
                },
                RecentWorkLogs = new List<WorkLogDto>(),
                RecentMaintenanceLogs = new List<MaintenanceLogDto>(),
                RecentEquipmentActions = new List<EquipmentActionDto>(),
                ActiveProjects = new List<ProjectDto>(),
                ProjectReports = new List<ProjectReportDto>()
            };

            var employees = await _employeeRepository.GetAsync(e => e.UserId == userId);
            var employee = employees.FirstOrDefault();
            if (employee != null)
            {
                dashboard.FullName = $"{employee.FirstName} {employee.LastName}";
            }

            if (roles != null && roles.Contains("Admin"))
            {
                dashboard.Stats = await GetSystemStatsAsync();
            }
            else if (roles != null && roles.Contains("Manager"))
            {
                dashboard.Stats = await GetManagerStatsAsync(userId);
            }
            else
            {
                dashboard.Stats = await GetDashboardStatsAsync(userId);
                dashboard.RecentWorkLogs = (await GetRecentWorkLogsAsync(userId, 5)).ToList();


                // Get recent maintenance logs
                dashboard.RecentMaintenanceLogs = (await GetRecentMaintenanceLogsAsync(userId, 5)).ToList();

                // Get recent equipment actions
                dashboard.RecentEquipmentActions = (await GetRecentEquipmentActionsAsync(userId, 5)).ToList();

                // Get active projects

                dashboard.ActiveProjects = (await GetUserProjectsAsync(userId)).ToList();
                dashboard.ProjectReports = (await GetProjectReportsAsync(userId)).ToList();
            }

            dashboard.UserRoles = roles?.ToList() ?? new List<string>();
            return dashboard;
        }

        private async Task<DashboardStatsDto> GetSystemStatsAsync()
        {
            var stats = new DashboardStatsDto();

            stats.TotalProjects = await _projectRepository.CountAsync();
            stats.ActiveProjects = await _projectRepository.CountAsync(p => p.Status == ProjectStatus.Active);
            stats.CompletedProjects = await _projectRepository.CountAsync(p => p.Status == ProjectStatus.Completed);

            var workLogsQuery = _workLogRepository.GetQueryable();
            stats.TotalWorkLogs = await workLogsQuery.CountAsync();

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            stats.TotalHoursThisMonth = await workLogsQuery
                .Where(w => w.WorkDate >= startOfMonth)
                .SumAsync(w => w.HoursSpent);

            var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            stats.TotalHoursThisWeek = await workLogsQuery
                .Where(w => w.WorkDate >= startOfWeek)
                .SumAsync(w => w.HoursSpent);

            return stats;
        }

        private async Task<DashboardStatsDto> GetManagerStatsAsync(int userId)
        {
            var stats = new DashboardStatsDto();

            var projects = await _projectRepository.GetAsync(
                p => p.ProjectEmployees.Any(pe => pe.Employee.UserId == userId && pe.Role == "Manager"),
                includes: new Expression<Func<Project, object>>[] { p => p.WorkLogs });

            stats.TotalProjects = projects.Count();
            stats.ActiveProjects = projects.Count(p => p.Status == ProjectStatus.Active);
            stats.CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed);

            var workLogs = projects.SelectMany(p => p.WorkLogs);
            stats.TotalWorkLogs = workLogs.Count();

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            stats.TotalHoursThisMonth = workLogs
                .Where(w => w.WorkDate >= startOfMonth)
                .Sum(w => w.HoursSpent);

            var startOfWeek = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
            stats.TotalHoursThisWeek = workLogs
                .Where(w => w.WorkDate >= startOfWeek)
                .Sum(w => w.HoursSpent);

            return stats;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync(int userId)
        {
            var stats = new DashboardStatsDto
            {
                WeeklyHours = new List<decimal>()
            };

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

                var projectIds = projectEmployees.Select(pe => pe.ProjectId).ToList();

                // Equipment count across user's projects
                stats.TotalEquipment = await _equipmentRepository.CountAsync(e => e.ProjectId.HasValue && projectIds.Contains(e.ProjectId.Value));

                // Get work logs stats
                var workLogs = await _workLogRepository.GetAsync(
                    w => w.EmployeeId == employee.Id,
                    includes: new Expression<Func<WorkLog, object>>[] { w => w.History });
                stats.TotalWorkLogs = workLogs.Count();
                stats.PendingWorkLogApprovals = workLogs.Count(w => !w.History.Any(h => h.Action == "Approved"));

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


                // Maintenance tasks for user's projects
                var maintenanceLogs = await _maintenanceLogRepository.GetAsync(
                    l => l.MaintenanceSchedule.Project.ProjectEmployees.Any(pe => pe.EmployeeId == employee.Id),
                    includes: new Expression<Func<MaintenanceLog, object>>[] { l => l.MaintenanceSchedule });
                stats.ActiveTasks = maintenanceLogs.Count(l => !l.IsCompleted);
                stats.CompletedTasks = maintenanceLogs.Count(l => l.IsCompleted);

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
                pe => pe.EmployeeId == employee.Id,
                includes: new Expression<Func<ProjectEmployee, object>>[]
                {
                    pe => pe.Project,
                    pe => pe.Project.WorkLogs
                });

            var projects = projectEmployees.Select(pe => pe.Project).Distinct();
            var projectDtos = new List<ProjectDto>();

            foreach (var project in projects)
            {
                var dto = _mapper.Map<ProjectDto>(project);

                var spent = project.WorkLogs.Sum(w => w.Cost);
                dto.CompletionPercent = project.Budget > 0 ? Math.Round(spent / project.Budget * 100, 2) : 0;
                dto.StatusText = project.Status switch
                {
                    ProjectStatus.Planning => "Planning",
                    ProjectStatus.Active => "Active",
                    ProjectStatus.OnHold => "On Hold",
                    ProjectStatus.Completed => "Completed",
                    ProjectStatus.Cancelled => "Cancelled",
                    _ => project.Status.ToString()
                };

                projectDtos.Add(dto);
            }

            return projectDtos;
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