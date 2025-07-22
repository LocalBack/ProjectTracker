using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System;
using System.Threading.Tasks;

namespace ProjectTracker.Admin.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly AppDbContext _context;

        public CreateModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public IActionResult OnGet()
        {
            Project = new Project
            {
                StartDate = DateTime.Today,
                Status = ProjectStatus.Active
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Projects == null || Project == null)
            {
                return Page();
            }

            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}