using System;

namespace ProjectTracker.Core.Entities
{
    public class ProjectEmployee
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public DateTime AssignedDate { get; set; }
        public DateTime? UnassignedDate { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}