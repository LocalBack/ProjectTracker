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

        public ProjectDashboardService(
            IRepository<Project> projectRepository,
            IRepository<WorkLog> workLogRepository,
            IRepository<MaintenanceSchedule> maintenanceRepository)
        {
            _projectRepository = projectRepository;
            _workLogRepository = workLogRepository;
            _maintenanceRepository = maintenanceRepository;
        }

        public async Task<ProjectSummaryDto> GetSummaryAsync(int projectId)
        {
            var projects = await _projectRepository.GetAsync(
                p => p.Id == projectId,
                includes: new Expression<Func<Project, object>>[]
                {
                    p => p.WorkLogs,
                    p => p.ProjectEmployees
                });

            var project = projects.FirstOrDefault();
            if (project == null)
                return new ProjectSummaryDto();

            var startOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            return new ProjectSummaryDto
            {
                Name = project.Name,
                Budget = project.Budget,
                Spent = project.WorkLogs.Sum(w => w.Cost),
                HoursTotal = project.WorkLogs.Sum(w => w.HoursSpent),
                HoursMonth = project.WorkLogs
                    .Where(w => w.WorkDate >= startOfMonth)
                    .Sum(w => w.HoursSpent),
                ActiveEmployeeCount = project.ProjectEmployees.Count()
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
