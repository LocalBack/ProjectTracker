namespace ProjectTracker.Service.DTOs
{
    public class ProjectSummaryDto
    {
        public string Name { get; set; }
        public decimal Budget { get; set; }
        public decimal Spent { get; set; }
        public decimal HoursMonth { get; set; }
        public decimal HoursTotal { get; set; }
        public int ActiveEmployeeCount { get; set; }
    }
}
