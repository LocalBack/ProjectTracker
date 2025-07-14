namespace ProjectTracker.Core.Entities
{
    public class WorkLogDetail : BaseEntity
    {
        public int WorkLogId { get; set; }
        public WorkLog WorkLog { get; set; } = null!;

        public int StepNumber { get; set; }
        public string StepDescription { get; set; } = string.Empty;
        public string TechnicalDetails { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? AdditionalData { get; set; }
    }
}