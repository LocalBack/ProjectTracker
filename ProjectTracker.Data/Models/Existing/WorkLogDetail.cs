using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class WorkLogDetail
{
    public int Id { get; set; }

    public int WorkLogId { get; set; }

    public int StepNumber { get; set; }

    public string StepDescription { get; set; } = null!;

    public string TechnicalDetails { get; set; } = null!;

    public string Result { get; set; } = null!;

    public string? AdditionalData { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual WorkLog WorkLog { get; set; } = null!;
}
