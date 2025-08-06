using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

namespace ProjectTracker.Admin.Pages.Projects
{
    public class EditModel : PageModel
    {
        private readonly AppDbContext _context;

        public EditModel(AppDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        [BindProperty]
        public IFormFile? Document { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(p => p.Documents).FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }
            Project = project;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projectToUpdate = await _context.Projects.Include(p => p.Documents).FirstOrDefaultAsync(p => p.Id == Project.Id);
            if (projectToUpdate == null)
            {
                return NotFound();
            }

            projectToUpdate.Name = Project.Name;
            projectToUpdate.Description = Project.Description;
            projectToUpdate.StartDate = Project.StartDate;
            projectToUpdate.EndDate = Project.EndDate;
            projectToUpdate.Budget = Project.Budget;
            projectToUpdate.ActualCost = Project.ActualCost;
            projectToUpdate.Status = Project.Status;

            if (Document != null && Document.Length > 50 * 1024 * 1024)
            {
                ModelState.AddModelError("Document", "Dosya boyutu 50 MB'dan büyük olamaz.");
                return Page();
            }

            if (Document != null && Document.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Path.GetFileName(Document.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Document.CopyToAsync(stream);
                }

                projectToUpdate.Documents = projectToUpdate.Documents ?? new List<ProjectDocument>();
                projectToUpdate.Documents.Add(new ProjectDocument
                {
                    FileName = fileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    FileType = Document.ContentType,
                    FileSize = Document.Length
                });
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(Project.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ProjectExists(int id)
        {
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}