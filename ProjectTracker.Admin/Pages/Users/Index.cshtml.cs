using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities; // ApplicationUser için doðru namespace
using ProjectTracker.Data.Context; // AppDbContext için doðru namespace

namespace ProjectTracker.Admin.Pages.Users
{
    [Authorize(Roles = "Admin")] // Sadece Admin rolüne sahip kullanýcýlar eriþebilir
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
            // Tüm ApplicationUser'larý çekiyoruz
            Users = await _userManager.Users.ToListAsync();
        }

        // Kullanýcýnýn belirli bir rolde olup olmadýðýný kontrol etmek için yardýmcý metod
        public async Task<bool> IsUserInRole(ApplicationUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }
    }
}