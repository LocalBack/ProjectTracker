using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using ProjectTracker.Service.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace ProjectTracker.Web.Controllers
{
    [Authorize]
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ILogger<ProjectController> _logger;

        public ProjectController(IProjectService projectService, ILogger<ProjectController> logger)
        {
            _projectService = projectService;
            _logger = logger;
        }

        // GET: Project
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? pageSize)
        {
            // ViewData ayarları
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["BudgetSortParm"] = sortOrder == "Budget" ? "budget_desc" : "Budget";

            // Arama
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentPageSize"] = pageSize ?? 10;

            // Query oluştur
            var projects = await _projectService.GetAllProjectsQueryableAsync();

            // Filtreleme
            if (!String.IsNullOrEmpty(searchString))
            {
                projects = projects.Where(s => s.Name.Contains(searchString)
                                       || s.Description.Contains(searchString));
            }

            // Sıralama
            switch (sortOrder)
            {
                case "name_desc":
                    projects = projects.OrderByDescending(s => s.Name);
                    break;
                case "Date":
                    projects = projects.OrderBy(s => s.StartDate);
                    break;
                case "date_desc":
                    projects = projects.OrderByDescending(s => s.StartDate);
                    break;
                case "Budget":
                    projects = projects.OrderBy(s => s.Budget);
                    break;
                case "budget_desc":
                    projects = projects.OrderByDescending(s => s.Budget);
                    break;
                default:
                    projects = projects.OrderBy(s => s.Name);
                    break;
            }

            int selectedPageSize = pageSize ?? 10;

            // PaginatedList oluştur
            var paginatedList = await PaginatedList<ProjectDto>.CreateAsync(
                projects,
                pageNumber ?? 1,
                selectedPageSize,
                searchString,
                sortOrder);

            return View(paginatedList);
        }

        // GET: Project/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectDto projectDto)
        {
            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectAsync(projectDto);
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProjectDto projectDto)
        {
            if (id != projectDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _projectService.UpdateProjectAsync(id, projectDto);
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var project = await _projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Project/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _projectService.DeleteProjectAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}