using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Web.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace ProjectTracker.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        // GET: Project
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            // Sorting parameters
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["StatusSortParm"] = sortOrder == "Status" ? "status_desc" : "Status";

            // Search/Filter
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            // Get all projects
            var projects = await _projectService.GetAllProjectsAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(p =>
                    p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description != null && p.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply sorting
            projects = sortOrder switch
            {
                "name_desc" => projects.OrderByDescending(p => p.Name).ToList(),
                "Date" => projects.OrderBy(p => p.StartDate).ToList(),
                "date_desc" => projects.OrderByDescending(p => p.StartDate).ToList(),
                "Status" => projects.OrderBy(p => p.Status).ToList(),
                "status_desc" => projects.OrderByDescending(p => p.Status).ToList(),
                _ => projects.OrderBy(p => p.Name).ToList(),
            };

            // Pagination
            int currentPageSize = pageSize ?? 10;
            ViewData["CurrentPageSize"] = currentPageSize;

            // Create paginated list
            var paginatedProjects = new PaginatedList<ProjectDto>(
                projects.Skip(((pageNumber ?? 1) - 1) * currentPageSize).Take(currentPageSize).ToList(),
                projects.Count(),
                pageNumber ?? 1,
                currentPageSize,
                searchString,
                sortOrder
            );

            return View(paginatedProjects);
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Project/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("Name,Description,StartDate,EndDate,Budget,Status")] ProjectDto projectDto, IFormFile? document)
        {
            if (document != null && document.Length > 50 * 1024 * 1024)
            {
                ModelState.AddModelError("document", "Dosya boyutu 50 MB'dan büyük olamaz.");
            }

            if (ModelState.IsValid)
            {
                if (document != null && document.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(document.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    projectDto.Documents.Add(new ProjectDocumentDto
                    {
                        FileName = fileName,
                        FilePath = $"/uploads/{uniqueFileName}",
                        FileType = document.ContentType,
                        FileSize = document.Length
                    });
                }

                await _projectService.CreateProjectAsync(projectDto);
                TempData["SuccessMessage"] = "Proje başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,StartDate,EndDate,Budget,ActualCost,Status")] ProjectDto projectDto, IFormFile? document)
        {
            if (id != projectDto.Id)
            {
                return NotFound();
            }

            if (document != null && document.Length > 50 * 1024 * 1024)
            {
                ModelState.AddModelError("document", "Dosya boyutu 50 MB'dan büyük olamaz.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (document != null && document.Length > 0)
                    {
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploadsFolder))
                            Directory.CreateDirectory(uploadsFolder);

                        var fileName = Path.GetFileName(document.FileName);
                        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await document.CopyToAsync(stream);
                        }

                        projectDto.Documents.Add(new ProjectDocumentDto
                        {
                            FileName = fileName,
                            FilePath = $"/uploads/{uniqueFileName}",
                            FileType = document.ContentType,
                            FileSize = document.Length
                        });
                    }

                    await _projectService.UpdateProjectAsync(id, projectDto);
                    TempData["SuccessMessage"] = "Proje başarıyla güncellendi.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Proje güncellenirken bir hata oluştu.";
                    return View(projectDto);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                TempData["SuccessMessage"] = "Proje başarıyla silindi.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Proje silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Project/Statistics
        public async Task<IActionResult> Statistics()
        {
            var projects = await _projectService.GetAllProjectsAsync();

            var stats = new ProjectStatisticsViewModel
            {
                TotalProjects = projects.Count(),
                ActiveProjects = projects.Count(p => p.Status == Core.Entities.ProjectStatus.Active),
                CompletedProjects = projects.Count(p => p.Status == Core.Entities.ProjectStatus.Completed),
                TotalBudget = projects.Sum(p => p.Budget),
                TotalActualCost = projects.Sum(p => p.ActualCost ?? 0)
            };

            return View(stats);
        }
    }
}