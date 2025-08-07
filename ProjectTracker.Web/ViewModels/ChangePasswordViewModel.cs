using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Required")]
        [StringLength(100, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre Tekrar")]
        [Compare("NewPassword", ErrorMessage = "Yeni şifreler eşleşmiyor.")]
        public string ConfirmPassword { get; set; }
    }
}