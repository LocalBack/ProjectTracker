using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System;
using System.Threading.Tasks;
=======
using ProjectTracker.Data.Context;
using ProjectTracker.Core.Entities;
using AutoMapper;
using ProjectTracker.Service.DTOs;
>>>>>>> change-tests

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;
<<<<<<< HEAD

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
=======
        private readonly IMapper _mapper;

        public CreateModel(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [BindProperty]
        public MaintenanceScheduleDto Task { get; set; } = new();
        public SelectList EquipmentOptions { get; set; } = default!;

        public async Task OnGetAsync()
        {
            var equipments = await _context.Equipments.ToListAsync();
            EquipmentOptions = new SelectList(equipments, "Id", "Name");
            Task.LastMaintenanceDate = DateTime.Today;
>>>>>>> change-tests
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
<<<<<<< HEAD
                EquipmentList = new SelectList(await _context.Equipments.ToListAsync(), "Id", "Name", Schedule.EquipmentId);
                return Page();
            }

            _context.MaintenanceSchedules.Add(Schedule);
            await _context.SaveChangesAsync();

=======
                await OnGetAsync();
                return Page();
            }

            var entity = _mapper.Map<MaintenanceSchedule>(Task);
            entity.NextMaintenanceDate = Task.LastMaintenanceDate.AddDays(Task.IntervalDays);
            _context.MaintenanceSchedules.Add(entity);
            await _context.SaveChangesAsync();
>>>>>>> change-tests
            return RedirectToPage("Index");
        }
    }
}
