namespace ProjectTracker.Service.DTOs
{
    public class RecentMaintenanceDto
    {
        public string EquipmentName { get; set; } = string.Empty;
        public System.DateTime LastMaintenanceDate { get; set; }
        public System.DateTime NextMaintenanceDate { get; set; }
    }
}
