using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Data.Context;
using ProjectTracker.Service.DTOs;
using AutoMapper;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
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
        }
    }
}
