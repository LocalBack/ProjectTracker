namespace ProjectTracker.Service.DTOs
{
    public class ProjectEmployeeDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? UnassignedDate { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}