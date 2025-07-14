namespace ProjectTracker.Service.DTOs
{
    public class WorkLogDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; }
        public decimal HoursSpent { get; set; }
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int DetailCount { get; set; }
        public int AttachmentCount { get; set; }
    }

    public class WorkLogDetailDto
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }
        public int StepNumber { get; set; }
        public string StepDescription { get; set; } = string.Empty;
        public string TechnicalDetails { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string? AdditionalData { get; set; }
    }

    public class WorkLogAttachmentDto
    {
        public int Id { get; set; }
        public int WorkLogId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
    }

    public class CreateWorkLogDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; } = DateTime.Now;
        public decimal HoursSpent { get; set; }
        public int ProjectId { get; set; }
        public int EmployeeId { get; set; }
    }
}