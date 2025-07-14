using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class WorkLogAttachment
{
    public int Id { get; set; }

    public int WorkLogId { get; set; }

    public string FileName { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public long FileSize { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual WorkLog WorkLog { get; set; } = null!;
}
