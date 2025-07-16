namespace ProjectTracker.Service.DTOs
{
    public class EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string Type { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
    }
}