using System.Collections.Generic;

namespace ProjectTracker.Service.DTOs
{
    public class DashboardDto
    {
        public ProfileInfoDto ProfileInfo { get; set; } = new();
        public WorkSummaryDto WorkSummary { get; set; } = new();
        public ProjectsDto Projects { get; set; } = new();
        public ActivitiesDto Activities { get; set; } = new();
        public NotificationsDto Notifications { get; set; } = new();
        public DashboardStatsDto Stats { get; set; } = new();
        public ExportsDto Exports { get; set; } = new();
    }

    public class ProfileInfoDto
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new();
    }

    public class WorkSummaryDto
    {
        public decimal ThisWeekHours { get; set; }
        public decimal ThisMonthHours { get; set; }
    }

    public class ProjectsDto
    {
        public List<ProjectDto> ActiveProjects { get; set; } = new();
    }

    public class ActivitiesDto
    {
        public List<WorkLogDto> RecentWorkLogs { get; set; } = new();
    }

    public class NotificationsDto
    {
        public List<string> Items { get; set; } = new();
    }

    public class ExportsDto
    {
        public List<ProjectReportDto> ProjectReports { get; set; } = new();
    }

    public class DashboardStatsDto
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public decimal TotalHoursThisMonth { get; set; }
        public decimal TotalHoursThisWeek { get; set; }
        public int TotalWorkLogs { get; set; }
    }
}

