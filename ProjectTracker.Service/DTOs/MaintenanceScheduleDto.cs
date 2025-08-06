namespace ProjectTracker.Service.DTOs
{
    public class MaintenanceScheduleDto
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; } = string.Empty;
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string MaintenanceType { get; set; } = string.Empty;
        public int IntervalDays { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string Instructions { get; set; } = string.Empty;
        public bool IsNotificationSent { get; set; }
    }
}