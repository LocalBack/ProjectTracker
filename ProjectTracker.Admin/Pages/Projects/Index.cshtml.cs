using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;

        public IndexModel(AppDbContext context)
        {
            _context = context;
        }

        public IList<Project> Projects { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Projects != null)
            {
                Projects = await _context.Projects.ToListAsync();
            }
        }
    }
}