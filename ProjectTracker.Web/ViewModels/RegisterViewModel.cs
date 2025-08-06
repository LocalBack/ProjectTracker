using System.ComponentModel.DataAnnotations;
using ProjectTracker.Web.Resources.Models;

namespace ProjectTracker.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessageResourceName = "FirstNameRequired", ErrorMessageResourceType = typeof(User))]
        [Display(Name = "FirstName", ResourceType = typeof(User))]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceName = "LastNameRequired", ErrorMessageResourceType = typeof(User))]
        [Display(Name = "LastName", ResourceType = typeof(User))]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceName = "EmailRequired", ErrorMessageResourceType = typeof(User))]
        [EmailAddress(ErrorMessageResourceName = "EmailInvalid", ErrorMessageResourceType = typeof(User))]
        [Display(Name = "Email", ResourceType = typeof(User))]
        public string Email { get; set; }

        [Required(ErrorMessageResourceName = "PasswordRequired", ErrorMessageResourceType = typeof(User))]
        [StringLength(100, ErrorMessageResourceName = "PasswordLength", ErrorMessageResourceType = typeof(User), MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(User))]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(User))]
        [Compare("Password", ErrorMessageResourceName = "PasswordMismatch", ErrorMessageResourceType = typeof(User))]
        public string ConfirmPassword { get; set; }

        [Display(Name = "KvkkAccepted", ResourceType = typeof(User))]

        public bool KvkkAccepted { get; set; }
    }
}