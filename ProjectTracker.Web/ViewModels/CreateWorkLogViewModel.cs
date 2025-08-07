using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTracker.Web.Resources;

namespace ProjectTracker.Web.ViewModels
{
    public class CreateWorkLogViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "Title", ResourceType = typeof(SharedResource))]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Description", ResourceType = typeof(SharedResource))]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "WorkDate", ResourceType = typeof(SharedResource))]
        [DataType(DataType.Date)]
        public DateTime WorkDate { get; set; } = DateTime.Today;

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "HoursSpent", ResourceType = typeof(SharedResource))]
        [Range(0.1, 24, ErrorMessageResourceName = "HoursRange", ErrorMessageResourceType = typeof(DataAnnotations))]
        public decimal HoursSpent { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "Project", ResourceType = typeof(SharedResource))]
        public int ProjectId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "Employee", ResourceType = typeof(SharedResource))]
        public int EmployeeId { get; set; }

        // Dropdown listeler için
        public SelectList? Projects { get; set; }
        public SelectList? Employees { get; set; }
    }
}
