using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

// Doðru Entity Namespace'lerini kullanýn
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Admin.Pages
{
    [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanýcýlar eriþebilir
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            // AppDbContext burada direkt olarak Dashboard için kullanýlmýyor,
            // bu nedenle _context enjeksiyonunu kaldýrdým. Ýstatistikler eklendiðinde
            // tekrar eklenebilir.
        }

        public void OnGet()
        {
            // Dashboard için baþlangýçta yapýlacak özel bir iþlem yok.
            // Ýstatistikler eklendiðinde burasý güncellenecektir.
        }
    }
}