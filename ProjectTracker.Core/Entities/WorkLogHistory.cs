using System;

namespace ProjectTracker.Core.Entities
{
    public class WorkLogHistory : BaseEntity
    {
        public int WorkLogId { get; set; }
        public WorkLog WorkLog { get; set; } = null!;
        public int ChangedByUserId { get; set; }
        public string ChangedByUserName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? Changes { get; set; }
    }
}
