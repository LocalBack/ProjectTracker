using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Service.DTOs
{
    public class WorkLogDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çalışma tarihi zorunludur")]
        [Display(Name = "Çalışma Tarihi")]
        public DateTime WorkDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Harcanan süre zorunludur")]
        [Display(Name = "Harcanan Süre (Saat)")]
        [Range(0.1, 24, ErrorMessage = "Süre 0.1 ile 24 saat arasında olmalıdır")]
        public decimal HoursSpent { get; set; }

        [Display(Name = "Harcanan Para")]
        [Range(0, double.MaxValue, ErrorMessage = "Maliyet negatif olamaz")]
        public decimal Cost { get; set; }

        [Required(ErrorMessage = "Proje seçimi zorunludur")]
        [Display(Name = "Proje")]
        public int ProjectId { get; set; }

        [Display(Name = "Proje Adı")]
        public string ProjectName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Çalışan seçimi zorunludur")]
        [Display(Name = "Çalışan")]
        public int EmployeeId { get; set; }

        [Display(Name = "Çalışan Adı")]
        public string EmployeeName { get; set; } = string.Empty;

        // İlişkili veriler
        public ICollection<WorkLogDetailDto> Details { get; set; } = new List<WorkLogDetailDto>();
        public ICollection<WorkLogAttachmentDto> Attachments { get; set; } = new List<WorkLogAttachmentDto>();
        public int DetailCount { get; set; }
        public int AttachmentCount { get; set; }
    }
}