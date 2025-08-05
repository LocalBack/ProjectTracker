using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Collections.Generic;
using System.Threading.Tasks;
=======
using ProjectTracker.Data.Context;
using ProjectTracker.Service.DTOs;
using AutoMapper;
>>>>>>> change-tests

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
<<<<<<< HEAD

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<MaintenanceSchedule> Schedules { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Schedules = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .ToListAsync();
=======
        private readonly IMapper _mapper;

        public IndexModel(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IList<MaintenanceScheduleDto> Tasks { get; set; } = new List<MaintenanceScheduleDto>();

        public async Task OnGetAsync()
        {
            var schedules = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .ToListAsync();
            Tasks = _mapper.Map<IList<MaintenanceScheduleDto>>(schedules);
>>>>>>> change-tests
        }
    }
}
