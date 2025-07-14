using Microsoft.AspNetCore.Identity;

namespace ProjectTracker.Core.Entities
{
    public class ApplicationRole : IdentityRole<int>
    {
        public string? Description { get; set; }
    }
}