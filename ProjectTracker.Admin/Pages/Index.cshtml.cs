using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;

// Do�ru Entity Namespace'lerini kullan�n
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Admin.Pages
{
    [Authorize(Roles = "Admin")] // Sadece Admin rol�ne sahip kullan�c�lar eri�ebilir
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            // AppDbContext burada direkt olarak Dashboard i�in kullan�lm�yor,
            // bu nedenle _context enjeksiyonunu kald�rd�m. �statistikler eklendi�inde
            // tekrar eklenebilir.
        }

        public void OnGet()
        {
            // Dashboard i�in ba�lang��ta yap�lacak �zel bir i�lem yok.
            // �statistikler eklendi�inde buras� g�ncellenecektir.
        }
    }
}