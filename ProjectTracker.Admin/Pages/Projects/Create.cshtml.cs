using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections.Generic;

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

        [BindProperty]
        public IFormFile? Document { get; set; }

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

                Project.Documents = Project.Documents ?? new List<ProjectDocument>();
                Project.Documents.Add(new ProjectDocument
                {
                    FileName = fileName,
                    FilePath = $"/uploads/{uniqueFileName}",
                    FileType = Document.ContentType,
                    FileSize = Document.Length
                });
            }

            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}