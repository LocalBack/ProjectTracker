using System;

namespace ProjectTracker.Core.Entities
{
    public class ProjectDocument : BaseEntity
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string? Description { get; set; }
    }
}
