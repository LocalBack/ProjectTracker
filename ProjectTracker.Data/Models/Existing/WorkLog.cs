using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class WorkLog
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime WorkDate { get; set; }

    public decimal HoursSpent { get; set; }

    public int ProjectId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;

    public virtual ICollection<WorkLogAttachment> WorkLogAttachments { get; set; } = new List<WorkLogAttachment>();

    public virtual ICollection<WorkLogDetail> WorkLogDetails { get; set; } = new List<WorkLogDetail>();
}
