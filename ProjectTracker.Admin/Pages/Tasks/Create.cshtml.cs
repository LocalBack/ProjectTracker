using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public MaintenanceSchedule Schedule { get; set; } = default!;

        public SelectList ProjectList { get; set; } = default!;
        public SelectList EquipmentList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            ProjectList = new SelectList(projects, "Id", "Name");
            var firstProjectId = projects.FirstOrDefault()?.Id;
            var equipments = await _context.Equipments
                .Where(e => e.ProjectId == firstProjectId)
                .ToListAsync();
            EquipmentList = new SelectList(equipments, "Id", "Name");
            Schedule = new MaintenanceSchedule
            {
                ProjectId = firstProjectId ?? 0,
                LastMaintenanceDate = DateTime.Today,
                NextMaintenanceDate = DateTime.Today
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var projects = await _context.Projects.ToListAsync();
                ProjectList = new SelectList(projects, "Id", "Name", Schedule.ProjectId);
                var equipments = await _context.Equipments
                    .Where(e => e.ProjectId == Schedule.ProjectId)
                    .ToListAsync();
                EquipmentList = new SelectList(equipments, "Id", "Name", Schedule.EquipmentId);
                return Page();
            }

            Schedule.NextMaintenanceDate = Schedule.LastMaintenanceDate.AddDays(Schedule.IntervalDays);
            _context.MaintenanceSchedules.Add(Schedule);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
