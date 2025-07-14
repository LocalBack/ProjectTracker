using System;
using System.Collections.Generic;

namespace ProjectTracker.Core.Entities
{
    public class WorkLog : BaseEntity
    {
        public WorkLog()
        {
            Attachments = new HashSet<WorkLogAttachment>();
            Details = new HashSet<WorkLogDetail>();
        }

        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime WorkDate { get; set; }
        public decimal HoursSpent { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public ICollection<WorkLogAttachment> Attachments { get; set; }
        public ICollection<WorkLogDetail> Details { get; set; }
    }
}