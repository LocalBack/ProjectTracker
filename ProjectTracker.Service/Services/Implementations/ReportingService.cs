using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Enums;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class ReportingService : IReportingService
    {
        private readonly IWorkLogService _workLogService;
        private readonly IUserDashboardService _dashboardService;

        public ReportingService(IWorkLogService workLogService, IUserDashboardService dashboardService)
        {
            _workLogService = workLogService;
            _dashboardService = dashboardService;
        }

        public async Task<byte[]> ExportWorkLogsAsync(int userId, ExportFormat format)
        {
            var workLogs = await _workLogService.GetWorkLogsByUserIdAsync(userId);
            if (format == ExportFormat.Excel)
                return ExportWorkLogsToExcel(workLogs);
            if (format == ExportFormat.Pdf)
                return ExportWorkLogsToPdf(workLogs);
            return Array.Empty<byte>();
        }

        public async Task<byte[]> ExportActivityAsync(int userId, ExportFormat format)
        {
            var projects = await _dashboardService.GetUserProjectsAsync(userId);
            if (format == ExportFormat.Excel)
                return ExportActivityToExcel(projects);
            if (format == ExportFormat.Pdf)
                return ExportActivityToPdf(projects);
            return Array.Empty<byte>();
        }

        public async Task<byte[]> ExportPerformanceAsync(int userId, ExportFormat format)
        {
            var stats = await _dashboardService.GetDashboardStatsAsync(userId);
            if (format == ExportFormat.Excel)
                return ExportPerformanceToExcel(stats);
            if (format == ExportFormat.Pdf)
                return ExportPerformanceToPdf(stats);
            return Array.Empty<byte>();
        }

        private byte[] ExportWorkLogsToExcel(IEnumerable<WorkLogDto> logs)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("WorkLogs");
            ws.Cell(1, 1).Value = "Date";
            ws.Cell(1, 2).Value = "Project";
            ws.Cell(1, 3).Value = "Hours";
            ws.Cell(1, 4).Value = "Cost";

            var row = 2;
            foreach (var log in logs)
            {
                ws.Cell(row, 1).Value = log.WorkDate;
                ws.Cell(row, 2).Value = log.ProjectName;
                ws.Cell(row, 3).Value = log.HoursSpent;
                ws.Cell(row, 4).Value = log.Cost;
                row++;
            }

            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private byte[] ExportWorkLogsToPdf(IEnumerable<WorkLogDto> logs)
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
                            c.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Date");
                            header.Cell().Text("Project");
                            header.Cell().Text("Hours");
                            header.Cell().Text("Cost");
                        });

                        foreach (var log in logs)
                        {
                            table.Cell().Text(log.WorkDate.ToShortDateString());
                            table.Cell().Text(log.ProjectName);
                            table.Cell().Text(log.HoursSpent.ToString());
                            table.Cell().Text(log.Cost.ToString());
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        private byte[] ExportActivityToExcel(IEnumerable<ProjectDto> projects)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Activity");
            ws.Cell(1, 1).Value = "Project";
            ws.Cell(1, 2).Value = "Start";
            ws.Cell(1, 3).Value = "End";
            ws.Cell(1, 4).Value = "Status";
            var row = 2;
            foreach (var p in projects)
            {
                ws.Cell(row, 1).Value = p.Name;
                ws.Cell(row, 2).Value = p.StartDate;
                ws.Cell(row, 3).Value = p.EndDate;
                ws.Cell(row, 4).Value = p.Status.ToString();
                row++;
            }
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private byte[] ExportActivityToPdf(IEnumerable<ProjectDto> projects)
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
                            c.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Project");
                            header.Cell().Text("Start");
                            header.Cell().Text("End");
                            header.Cell().Text("Status");
                        });

                        foreach (var p in projects)
                        {
                            table.Cell().Text(p.Name);
                            table.Cell().Text(p.StartDate.ToShortDateString());
                            table.Cell().Text(p.EndDate?.ToShortDateString() ?? string.Empty);
                            table.Cell().Text(p.Status.ToString());
                        }
                    });
                });
            });
            return document.GeneratePdf();
        }

        private byte[] ExportPerformanceToExcel(DashboardStatsDto stats)
        {
            using var wb = new XLWorkbook();
            var ws = wb.AddWorksheet("Performance");
            ws.Cell(1, 1).Value = "Metric";
            ws.Cell(1, 2).Value = "Value";
            var data = new Dictionary<string, object>
            {
                {"Total Projects", stats.TotalProjects},
                {"Active Projects", stats.ActiveProjects},
                {"Completed Projects", stats.CompletedProjects},
                {"Total Hours This Month", stats.TotalHoursThisMonth},
                {"Total Hours This Week", stats.TotalHoursThisWeek},
                {"Total Work Logs", stats.TotalWorkLogs}
            };
            var row = 2;
            foreach (var item in data)
            {
                ws.Cell(row, 1).Value = item.Key;
                ws.Cell(row, 2).Value = item.Value?.ToString() ?? string.Empty;
                row++;
            }
            using var ms = new MemoryStream();
            wb.SaveAs(ms);
            return ms.ToArray();
        }

        private byte[] ExportPerformanceToPdf(DashboardStatsDto stats)
        {
            var data = new Dictionary<string, string>
            {
                {"Total Projects", stats.TotalProjects.ToString()},
                {"Active Projects", stats.ActiveProjects.ToString()},
                {"Completed Projects", stats.CompletedProjects.ToString()},
                {"Total Hours This Month", stats.TotalHoursThisMonth.ToString()},
                {"Total Hours This Week", stats.TotalHoursThisWeek.ToString()},
                {"Total Work Logs", stats.TotalWorkLogs.ToString()}
            };

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
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Metric");
                            header.Cell().Text("Value");
                        });

                        foreach (var item in data)
                        {
                            table.Cell().Text(item.Key);
                            table.Cell().Text(item.Value);
                        }
                    });
                });
            });
            return document.GeneratePdf();
        }
    }
}
