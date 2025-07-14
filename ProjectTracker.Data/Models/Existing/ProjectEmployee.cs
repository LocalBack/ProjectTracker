using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class ProjectEmployee
{
    public int ProjectId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime AssignedDate { get; set; }

    public DateTime? UnassignedDate { get; set; }

    public string Role { get; set; } = null!;

    public bool IsActive { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
