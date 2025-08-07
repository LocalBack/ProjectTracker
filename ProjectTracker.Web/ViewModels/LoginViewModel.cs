using System.ComponentModel.DataAnnotations;
using ProjectTracker.Web.Resources;

namespace ProjectTracker.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "Email", ResourceType = typeof(SharedResource))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(SharedResource))]
        public string Password { get; set; }

        [Display(Name = "RememberMe", ResourceType = typeof(SharedResource))]
        public bool RememberMe { get; set; }
    }
}
