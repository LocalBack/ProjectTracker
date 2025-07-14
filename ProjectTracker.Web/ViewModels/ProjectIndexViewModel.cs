using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.ViewModels
{
    public class ProjectIndexViewModel
    {
        public IEnumerable<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; }
    }

    public class ProjectDetailsViewModel
    {
        public ProjectDto Project { get; set; } = null!;
        public IEnumerable<WorkLogDto> RecentWorkLogs { get; set; } = new List<WorkLogDto>();
        public IEnumerable<EmployeeDto> TeamMembers { get; set; } = new List<EmployeeDto>();
    }
}