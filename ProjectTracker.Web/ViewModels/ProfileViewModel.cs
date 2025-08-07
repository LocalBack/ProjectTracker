using System.ComponentModel.DataAnnotations;
using ProjectTracker.Web.Resources;

namespace ProjectTracker.Web.ViewModels
{
    public class ProfileViewModel
    {
        [Display(Name = "FirstName", ResourceType = typeof(SharedResource))]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(SharedResource))]
        public string LastName { get; set; }

        [Display(Name = "Email", ResourceType = typeof(SharedResource))]
        public string Email { get; set; }

        [Display(Name = "RegistrationDate", ResourceType = typeof(SharedResource))]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "LastLogin", ResourceType = typeof(SharedResource))]
        public DateTime? LastLoginDate { get; set; }
    }
}
