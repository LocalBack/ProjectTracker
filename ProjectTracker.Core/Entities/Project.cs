using System;
using System.Collections.Generic;

namespace ProjectTracker.Core.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            WorkLogs = new HashSet<WorkLog>();
            ProjectEmployees = new HashSet<ProjectEmployee>();
            Equipments = new HashSet<Equipment>(); // Yeni eklendi
            MaintenanceSchedules = new HashSet<MaintenanceSchedule>();
            Documents = new HashSet<ProjectDocument>();
        }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Budget { get; set; }
        public decimal? ActualCost { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public ICollection<WorkLog> WorkLogs { get; set; }
        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public virtual ICollection<Equipment> Equipments { get; set; } = [];// Yeni eklendi
        public virtual ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = [];
        public virtual ICollection<ProjectDocument> Documents { get; set; } = [];
    }
}