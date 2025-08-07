namespace ProjectTracker.Service.DTOs
{
    public class ProjectSummaryDto
    {
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public System.DateTime StartDate { get; set; }
        public System.DateTime? EndDate { get; set; }
        public decimal HoursMonth { get; set; }
        public decimal HoursTotal { get; set; }
        public int ActiveEmployeeCount { get; set; }
        public int EquipmentCount { get; set; }
        public decimal CompletionPercent { get; set; }
    }
}
