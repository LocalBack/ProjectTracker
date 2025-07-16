namespace ProjectTracker.Service.DTOs
{
    public class WorkLogAttachmentDto
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}