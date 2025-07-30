using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class DetailsModel : PageModel
    {
        private readonly AppDbContext _context;

        public DetailsModel(AppDbContext context)
        {
            _context = context;
        }

        public MaintenanceSchedule Schedule { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Schedule = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (Schedule == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
