using Microsoft.AspNetCore.Identity;

namespace ProjectTracker.Core.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int? EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }

        // User-Project ilişkisi için
        public virtual ICollection<UserProject> UserProjects { get; set; } = new HashSet<UserProject>();

        public string FullName => $"{FirstName} {LastName}";
    }
}