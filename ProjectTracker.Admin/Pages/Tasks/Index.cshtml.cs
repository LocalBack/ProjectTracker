using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;

namespace ProjectTracker.Admin.Pages.Tasks
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<MaintenanceSchedule> Schedules { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Schedules = await _context.MaintenanceSchedules
                .Include(m => m.Equipment)
                .Include(m => m.Project)
                .ToListAsync();
        }
    }
}
