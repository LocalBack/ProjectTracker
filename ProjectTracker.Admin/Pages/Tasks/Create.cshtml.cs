using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System;
using System.Threading.Tasks;

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

        public SelectList EquipmentList { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            EquipmentList = new SelectList(await _context.Equipments.ToListAsync(), "Id", "Name");
            Schedule = new MaintenanceSchedule
            {
                LastMaintenanceDate = DateTime.Today,
                NextMaintenanceDate = DateTime.Today
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                EquipmentList = new SelectList(await _context.Equipments.ToListAsync(), "Id", "Name", Schedule.EquipmentId);
                return Page();
            }

            _context.MaintenanceSchedules.Add(Schedule);
            await _context.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
