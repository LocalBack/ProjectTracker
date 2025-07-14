using System;
using System.Collections.Generic;

namespace ProjectTracker.Core.Entities
{
    public class Employee : BaseEntity
    {
        public Employee()
        {
            ProjectEmployees = new HashSet<ProjectEmployee>();
            WorkLogs = new HashSet<WorkLog>();
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public DateTime HireDate { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        public ICollection<ProjectEmployee> ProjectEmployees { get; set; }
        public ICollection<WorkLog> WorkLogs { get; set; }
    }
}