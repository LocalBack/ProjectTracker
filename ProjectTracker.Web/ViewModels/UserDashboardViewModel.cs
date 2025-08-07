using System.Collections.Generic;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.ViewModels
{
    public class UserDashboardViewModel
    {
        public ProfileInfoViewModel ProfileInfo { get; set; } = new();
        public WorkSummaryViewModel WorkSummary { get; set; } = new();
        public ProjectsViewModel Projects { get; set; } = new();
        public ActivitiesViewModel Activities { get; set; } = new();
        public NotificationsViewModel Notifications { get; set; } = new();
        public DashboardStatsDto Stats { get; set; } = new();
        public ExportsViewModel Exports { get; set; } = new();
    }

    public class ProfileInfoViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string> UserRoles { get; set; } = new();
    }

    public class WorkSummaryViewModel
    {
        public decimal ThisWeekHours { get; set; }
        public decimal ThisMonthHours { get; set; }
    }

    public class ProjectsViewModel
    {
        public List<ProjectDto> ActiveProjects { get; set; } = new();
    }

    public class ActivitiesViewModel
    {
        public List<WorkLogDto> RecentWorkLogs { get; set; } = new();
    }

    public class NotificationsViewModel
    {
        public List<string> Items { get; set; } = new();
    }

    public class ExportsViewModel
    {
        public List<ProjectReportDto> ProjectReports { get; set; } = new();
    }
}

