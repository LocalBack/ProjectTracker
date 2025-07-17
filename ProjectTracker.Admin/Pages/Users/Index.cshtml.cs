using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities; // ApplicationUser i�in do�ru namespace
using ProjectTracker.Data.Context; // AppDbContext i�in do�ru namespace

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")] // Sadece Admin rol�ne sahip kullan�c�lar eri�ebilir
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _context; // DbContext'i enjekte ediyoruz

        public IndexModel(UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IList<ApplicationUser> Users { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // T�m ApplicationUser'lar� �ekiyoruz
            Users = await _userManager.Users.ToListAsync();
        }

        // Kullan�c�n�n belirli bir rolde olup olmad���n� kontrol etmek i�in yard�mc� metod
        public async Task<bool> IsUserInRole(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }
    }
}