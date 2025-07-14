using System;
using System.Collections.Generic;

namespace ProjectTracker.Data.Models.Existing;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Title { get; set; } = null!;

    public DateTime HireDate { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; } = new List<ProjectEmployee>();

    public virtual ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
}
