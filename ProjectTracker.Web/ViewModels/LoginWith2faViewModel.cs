using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Web.ViewModels
{
    public class LoginWith2faViewModel
    {
        [Required(ErrorMessage = "Doğrulama kodu zorunludur")]
        [StringLength(7, ErrorMessage = "{0} {2} ile {1} karakter arasında olmalıdır.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Doğrulama Kodu")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "Bu cihazı hatırla")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}