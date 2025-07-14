namespace ProjectTracker.Core.Entities
{
    public class WorkLogAttachment : BaseEntity
    {
        public int WorkLogId { get; set; }
        public WorkLog WorkLog { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
    }
}