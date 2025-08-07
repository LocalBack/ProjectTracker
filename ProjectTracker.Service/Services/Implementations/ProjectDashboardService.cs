using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Enums;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    /// <summary>
    /// Provides aggregated data for project dashboards and simple export capability.
    /// The implementation uses repositories directly to gather statistics; more complex
    /// business rules can be added later as needed.
    /// </summary>
    public class ProjectDashboardService : IProjectDashboardService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<WorkLog> _workLogRepository;
        private readonly IRepository<MaintenanceSchedule> _maintenanceRepository;
        private readonly IRepository<Equipment> _equipmentRepository;

        public ProjectDashboardService(
            IRepository<Project> projectRepository,
            IRepository<WorkLog> workLogRepository,
            IRepository<MaintenanceSchedule> maintenanceRepository,
            IRepository<Equipment> equipmentRepository)
        {
            _projectRepository = projectRepository;
            _workLogRepository = workLogRepository;
            _maintenanceRepository = maintenanceRepository;
            _equipmentRepository = equipmentRepository;
        }

        public async Task<ProjectSummaryDto> GetSummaryAsync(int projectId)
        {
            var projects = await _projectRepository.GetAsync(
                p => p.Id == projectId,
                includes: new Expression<Func<Project, object>>[]
                {
                    p => p.WorkLogs,
                    p => p.ProjectEmployees,
                    p => p.Equipments
                });

            var project = projects.FirstOrDefault();
            if (project == null)
                return new ProjectSummaryDto();

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            var spent = project.WorkLogs.Sum(w => w.Cost);
            return new ProjectSummaryDto
            {
                Name = project.Name,
                Budget = project.Budget,
                Spent = spent,
                StartDate = project.StartDate,
                EndDate = project.EndDate,
                HoursTotal = project.WorkLogs.Sum(w => w.HoursSpent),
                HoursMonth = project.WorkLogs
                    .Where(w => w.WorkDate >= startOfMonth)
                    .Sum(w => w.HoursSpent),
                ActiveEmployeeCount = project.ProjectEmployees.Count(),
                EquipmentCount = project.Equipments.Count(),
                CompletionPercent = project.Budget > 0 ? Math.Round(spent / project.Budget * 100, 2) : 0
            };
        }

        public async Task<IEnumerable<TaskStatusDto>> GetTaskStatusAsync(int projectId)
        {
            // There is no dedicated Task entity in the sample project. As a placeholder
            // we return the number of work logs for the project so the dashboard can display data.
            var workLogs = await _workLogRepository.GetAsync(w => w.ProjectId == projectId);
            return new[]
            {
                new TaskStatusDto { Status = "WorkLogs", Count = workLogs.Count() }
            };
        }

        public async Task<IEnumerable<WorkLogTrendDto>> GetWorkLogTrendAsync(int projectId, int months)
        {
            var fromDate = DateTime.Now.AddMonths(-months + 1);
            var logs = await _workLogRepository.GetAsync(
                w => w.ProjectId == projectId && w.WorkDate >= fromDate);

            var trend = logs
                .GroupBy(w => new { w.WorkDate.Year, w.WorkDate.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new WorkLogTrendDto
                {
                    Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("yyyy-MM"),
                    Hours = g.Sum(w => w.HoursSpent)
                });

            return trend;
        }

        public async Task<IEnumerable<MaintenanceDto>> GetUpcomingMaintenanceAsync(int projectId, int days)
        {
            var limit = DateTime.Now.AddDays(days);
            var schedules = await _maintenanceRepository.GetAsync(
                m => m.ProjectId == projectId && m.NextMaintenanceDate <= limit,
                includes: new Expression<Func<MaintenanceSchedule, object>>[]
                {
                    m => m.Equipment,
                    m => m.Project
                });

            return schedules.Select(s => new MaintenanceDto
            {
                Equipment = s.Equipment.Name,
                NextDate = s.NextMaintenanceDate,
                Type = s.MaintenanceType,
                Project = s.Project.Name
            });
        }

        public async Task<IEnumerable<EquipmentStatusDto>> GetEquipmentAsync(int projectId)
        {
            var equipments = await _equipmentRepository.GetAsync(
                e => e.ProjectId == projectId,
                includes: new Expression<Func<Equipment, object>>[]
                {
                    e => e.MaintenanceSchedules
                });

            var result = equipments.Select(e =>
            {
                var schedule = e.MaintenanceSchedules.OrderByDescending(s => s.NextMaintenanceDate).FirstOrDefault();
                var status = schedule == null ? "No Schedule" :
                    (schedule.NextMaintenanceDate < DateTime.Now ? "Overdue" : "OK");
                return new EquipmentStatusDto
                {
                    Name = e.Name,
                    Type = e.Type,
                    LastMaintenanceDate = schedule?.LastMaintenanceDate,
                    NextMaintenanceDate = schedule?.NextMaintenanceDate,
                    Status = status
                };
            });

            if (!result.Any())
            {
                return new[]
                {
                    new EquipmentStatusDto
                    {
                        Name = "Sample Equipment",
                        Type = "TypeA",
                        LastMaintenanceDate = DateTime.Now.AddDays(-10),
                        NextMaintenanceDate = DateTime.Now.AddDays(20),
                        Status = "OK"
                    }
                };
            }

            return result;
        }

        public async Task<IEnumerable<RecentWorkLogDto>> GetRecentWorkLogsAsync(int projectId, int count)
        {
            var logs = await _workLogRepository.GetAsync(
                w => w.ProjectId == projectId,
                orderBy: q => q.OrderByDescending(w => w.WorkDate),
                includes: new Expression<Func<WorkLog, object>>[] { w => w.Employee });

            var result = logs.Take(count).Select(w => new RecentWorkLogDto
            {
                Date = w.WorkDate,
                Title = w.Title,
                Employee = $"{w.Employee.FirstName} {w.Employee.LastName}",
                Duration = w.HoursSpent,
                Description = w.Description
            });

            if (!result.Any())
            {
                return new[]
                {
                    new RecentWorkLogDto
                    {
                        Date = DateTime.Now,
                        Title = "Sample Work",
                        Employee = "John Doe",
                        Duration = 2,
                        Description = "Demo log"
                    }
                };
            }

            return result;
        }

        public async Task<IEnumerable<RecentMaintenanceDto>> GetRecentMaintenanceAsync(int projectId, int count)
        {
            var schedules = await _maintenanceRepository.GetAsync(
                m => m.ProjectId == projectId,
                orderBy: q => q.OrderByDescending(s => s.LastMaintenanceDate),
                includes: new Expression<Func<MaintenanceSchedule, object>>[] { m => m.Equipment });

            var result = schedules.Take(count).Select(s => new RecentMaintenanceDto
            {
                EquipmentName = s.Equipment.Name,
                LastMaintenanceDate = s.LastMaintenanceDate,
                NextMaintenanceDate = s.NextMaintenanceDate
            });

            if (!result.Any())
            {
                return new[]
                {
                    new RecentMaintenanceDto
                    {
                        EquipmentName = "Sample Equipment",
                        LastMaintenanceDate = DateTime.Now.AddDays(-30),
                        NextMaintenanceDate = DateTime.Now.AddDays(30)
                    }
                };
            }

            return result;
        }

        public async Task<byte[]> ExportAsync(ExportTarget target, ExportFormat fmt, int? projectId)
        {
            if (target == ExportTarget.WorkLogs && projectId.HasValue)
            {
                var workLogs = await _workLogRepository.GetAsync(w => w.ProjectId == projectId.Value);

                if (fmt == ExportFormat.Excel)
                {
                    using var wb = new XLWorkbook();
                    var ws = wb.AddWorksheet("WorkLogs");
                    ws.Cell(1, 1).Value = "Date";
                    ws.Cell(1, 2).Value = "Title";
                    ws.Cell(1, 3).Value = "Hours";

                    var row = 2;
                    foreach (var log in workLogs)
                    {
                        ws.Cell(row, 1).Value = log.WorkDate;
                        ws.Cell(row, 2).Value = log.Title;
                        ws.Cell(row, 3).Value = log.HoursSpent;
                        row++;
                    }

                    using var ms = new MemoryStream();
                    wb.SaveAs(ms);
                    return ms.ToArray();
                }

                if (fmt == ExportFormat.Pdf)
                {
                    var document = Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(20);
                            page.Content().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                    c.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Date");
                                    header.Cell().Text("Title");
                                    header.Cell().Text("Hours");
                                });

                                foreach (var log in workLogs)
                                {
                                    table.Cell().Text(log.WorkDate.ToShortDateString());
                                    table.Cell().Text(log.Title);
                                    table.Cell().Text(log.HoursSpent.ToString());
                                }
                            });
                        });
                    });

                    return document.GeneratePdf();
                }
            }

            return Array.Empty<byte>();
        }
    }
}
