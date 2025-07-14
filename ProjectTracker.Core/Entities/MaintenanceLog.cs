namespace ProjectTracker.Core.Entities
{
    public class MaintenanceLog : BaseEntity
    {
        public int MaintenanceScheduleId { get; set; }
        public virtual MaintenanceSchedule MaintenanceSchedule { get; set; } = null!;

        public DateTime MaintenanceDate { get; set; }
        public string PerformedBy { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public decimal? Cost { get; set; }
        public bool IsCompleted { get; set; }
    }
}