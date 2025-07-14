using System.ComponentModel.DataAnnotations;

namespace ProjectTracker.Web.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "Ad")]
        public string FirstName { get; set; }

        [Display(Name = "Soyad")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Kayıt Tarihi")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Son Giriş")]
        public DateTime? LastLoginDate { get; set; }
    }
}