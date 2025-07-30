using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class DeleteModel : PageModel
    {
        private readonly AppDbContext _context;

        public DeleteModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var schedule = await _context.MaintenanceSchedules.FindAsync(id);
            if (schedule != null)
            {
                _context.MaintenanceSchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("Index");
        }
    }
}
