using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class Project
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal Budget { get; set; }

    public decimal? ActualCost { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
