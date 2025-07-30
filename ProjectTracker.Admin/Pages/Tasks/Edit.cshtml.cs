using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MaintenanceSchedule Schedule { get; set; } = default!;

        public SelectList EquipmentList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Schedule = await _context.MaintenanceSchedules.FirstOrDefaultAsync(m => m.Id == id);

            if (Schedule == null)
            {
                return NotFound();
            }

            EquipmentList = new SelectList(await _context.Equipments.ToListAsync(), "Id", "Name", Schedule.EquipmentId);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                EquipmentList = new SelectList(await _context.Equipments.ToListAsync(), "Id", "Name", Schedule.EquipmentId);
                return Page();
            }

            _context.Attach(Schedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ScheduleExists(Schedule.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("Index");
        }

        private bool ScheduleExists(int id)
        {
            return _context.MaintenanceSchedules.Any(e => e.Id == id);
        }
    }
}
