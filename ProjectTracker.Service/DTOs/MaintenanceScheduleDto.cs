namespace ProjectTracker.Service.DTOs
{
    public class MaintenanceScheduleDto
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int EquipmentId { get; set; }
        public string EquipmentName { get; set; }
        public string MaintenanceType { get; set; }
        public int IntervalDays { get; set; }
        public DateTime LastMaintenanceDate { get; set; }
        public DateTime NextMaintenanceDate { get; set; }
        public string Instructions { get; set; }
        public bool IsNotificationSent { get; set; }
    }
}