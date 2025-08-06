using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Data.Context;
using ProjectTracker.Core.Entities;
using AutoMapper;
using ProjectTracker.Service.DTOs;
using System.Linq;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CreateModel(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
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
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
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
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
