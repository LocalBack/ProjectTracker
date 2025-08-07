namespace ProjectTracker.Service.DTOs
{
    public class EquipmentStatusDto
    {
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public System.DateTime? LastMaintenanceDate { get; set; }
        public System.DateTime? NextMaintenanceDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
