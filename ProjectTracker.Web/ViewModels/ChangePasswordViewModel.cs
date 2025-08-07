using System.ComponentModel.DataAnnotations;
using ProjectTracker.Web.Resources;

namespace ProjectTracker.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof(SharedResource))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DataAnnotations), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(SharedResource))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmNewPassword", ResourceType = typeof(SharedResource))]
        [Compare("NewPassword", ErrorMessageResourceName = "PasswordMismatch", ErrorMessageResourceType = typeof(DataAnnotations))]
        public string ConfirmPassword { get; set; }
    }
}
