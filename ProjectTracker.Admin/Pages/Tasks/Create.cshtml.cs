using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
<<<<<<< HEAD
using ProjectTracker.Data.Context;
=======
using AutoMapper;
using ProjectTracker.Service.DTOs;
using System.Linq;
>>>>>>> 2087c97b6f0cb1ceca3b91bce13a5f84bb9a351d

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
<<<<<<< HEAD
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
=======
        public MaintenanceScheduleDto Task { get; set; } = new();
        public SelectList EquipmentOptions { get; set; } = default!;
        public SelectList ProjectOptions { get; set; } = default!;

        public async Task OnGetAsync(int? projectId)
        {
            var projects = await _context.Projects.ToListAsync();
            ProjectOptions = new SelectList(projects, "Id", "Name");
            Task.ProjectId = projectId ?? (projects.FirstOrDefault()?.Id ?? 0);

            var equipments = await _context.Equipments
                .Where(e => e.ProjectId == Task.ProjectId)
                .ToListAsync();
            EquipmentOptions = new SelectList(equipments, "Id", "Name");
            Task.LastMaintenanceDate = DateTime.Today;
>>>>>>> 2087c97b6f0cb1ceca3b91bce13a5f84bb9a351d
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
<<<<<<< HEAD
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
=======
                await OnGetAsync(Task.ProjectId);
                return Page();
            }

            var equipment = await _context.Equipments.FindAsync(Task.EquipmentId);
            if (equipment == null)
            {
                ModelState.AddModelError("Task.EquipmentId", "Invalid equipment selected.");
                await OnGetAsync(Task.ProjectId);
                return Page();
            }
            Task.ProjectId = equipment.ProjectId ?? Task.ProjectId;

            var entity = _mapper.Map<MaintenanceSchedule>(Task);
            entity.NextMaintenanceDate = Task.LastMaintenanceDate.AddDays(Task.IntervalDays);
            _context.MaintenanceSchedules.Add(entity);
>>>>>>> 2087c97b6f0cb1ceca3b91bce13a5f84bb9a351d
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
