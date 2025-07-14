namespace ProjectTracker.Core.Entities
{
    public class UserProject
    {
        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; } = null!;

        public DateTime AssignedDate { get; set; }
        public bool CanView { get; set; } = true;
        public bool CanEdit { get; set; } = false;
        public bool CanDelete { get; set; } = false;
    }
}