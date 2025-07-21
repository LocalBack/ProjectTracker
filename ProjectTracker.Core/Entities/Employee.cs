using ProjectTracker.Core.Entities;
using System.ComponentModel.DataAnnotations;

public class Employee : BaseEntity
{
    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }  // Changed from Name

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }   // Added

    [StringLength(100)]
    public string Title { get; set; }      // Changed from Position

    [StringLength(50)]
    public string EmployeeCode { get; set; }

    [StringLength(100)]
    public string Department { get; set; }

    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Phone]
    [StringLength(20)]
    public string Phone { get; set; }

    public DateTime HireDate { get; set; }

    // Link to user account
    public int? UserId { get; set; }
    public virtual ApplicationUser User { get; set; }

    // Navigation properties
    public virtual ICollection<ProjectEmployee> ProjectEmployees { get; set; }
    public virtual ICollection<WorkLog> WorkLogs { get; set; }

    public Employee()
    {
        ProjectEmployees = new HashSet<ProjectEmployee>();
        WorkLogs = new HashSet<WorkLog>();
    }
}