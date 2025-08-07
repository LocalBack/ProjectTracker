namespace ProjectTracker.Core.Entities
{
    public class MaintenanceSchedule : BaseEntity
    {
        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; } = null!;

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public string MaintenanceType { get; set; } = string.Empty;
        public int IntervalDays { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string? Instructions { get; set; }
        public bool IsNotificationSent { get; set; }

        public virtual ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new HashSet<MaintenanceLog>();
    }
}