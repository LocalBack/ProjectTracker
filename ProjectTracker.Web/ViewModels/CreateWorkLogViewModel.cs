using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectTracker.Web.ViewModels
{
    public class CreateWorkLogViewModel
    {
        [Required(ErrorMessage = "Başlık zorunludur")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çalışma tarihi zorunludur")]
        [Display(Name = "Çalışma Tarihi")]
        [DataType(DataType.Date)]
        public DateTime WorkDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Harcanan süre zorunludur")]
        [Display(Name = "Harcanan Süre (Saat)")]
        [Range(0.1, 24, ErrorMessage = "Süre 0.1 ile 24 saat arasında olmalıdır")]
        public decimal HoursSpent { get; set; }

        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        [Display(Name = "Proje")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Çalışan seçimi zorunludur")]
        [Display(Name = "Çalışan")]
        public int EmployeeId { get; set; }

        // Dropdown listeler için
        public SelectList? Projects { get; set; }
        public SelectList? Employees { get; set; }
    }
}