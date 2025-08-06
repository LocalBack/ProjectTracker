using System;

namespace ProjectTracker.Service.DTOs
{
    public class ProjectReportDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public decimal TotalCost { get; set; }
        public decimal Budget { get; set; }
        public decimal ActualCost { get; set; }
    }
}
