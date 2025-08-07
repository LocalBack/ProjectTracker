using System.ComponentModel.DataAnnotations;
using ProjectTracker.Web.Resources;

namespace ProjectTracker.Web.ViewModels
{
    public class LoginWith2faViewModel
    {
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [StringLength(7, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DataAnnotations), MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "VerificationCode", ResourceType = typeof(SharedResource))]
        public string TwoFactorCode { get; set; }

        [Display(Name = "RememberThisDevice", ResourceType = typeof(SharedResource))]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }
    }
}
