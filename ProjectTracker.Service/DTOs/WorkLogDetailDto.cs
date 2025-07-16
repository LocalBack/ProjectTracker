namespace ProjectTracker.Service.DTOs
{
    public class WorkLogDetailDto
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; } = string.Empty;
        public string TechnicalDetails { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string AdditionalData { get; set; } = string.Empty;
    }
}