namespace ProjectTracker.Core.Entities
{
    public class Equipment : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime PurchaseDate { get; set; }
        public int? ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public virtual ICollection<MaintenanceSchedule> MaintenanceSchedules { get; set; } = new HashSet<MaintenanceSchedule>();
    }
}