using ProjectTracker.Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ProjectTracker.Service.DTOs
{
    public class ProjectDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Proje adı zorunludur")]
        [Display(Name = "Proje Adı")]
        [StringLength(200, ErrorMessage = "Proje adı en fazla 200 karakter olabilir")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Başlangıç tarihi zorunludur")]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "Bütçe zorunludur")]
        [Display(Name = "Bütçe")]
        [Range(0, double.MaxValue, ErrorMessage = "Bütçe 0'dan büyük olmalıdır")]
        public decimal Budget { get; set; }

        [Display(Name = "Gerçekleşen Maliyet")]
        public decimal? ActualCost { get; set; }
        public ProjectStatus Status { get; set; } = ProjectStatus.Active;
        public ICollection<ProjectDocumentDto> Documents { get; set; } = new List<ProjectDocumentDto>();
    }
}