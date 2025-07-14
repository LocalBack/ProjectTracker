using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.Controllers
{
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
        public async Task<IActionResult> Index()
        {
            try
            {
                var projects = await _projectService.GetActiveProjectsAsync();
                return View(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting projects");
                TempData["Error"] = "Projeler yüklenirken hata oluştu.";
                return View(new List<ProjectDto>());
            }
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
        public async Task<IActionResult> Create(CreateProjectDto projectDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _projectService.CreateProjectAsync(projectDto);
                    TempData["Success"] = "Proje başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating project");
                    ModelState.AddModelError("", "Proje oluşturulurken hata oluştu.");
                }
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
                try
                {
                    await _projectService.UpdateProjectAsync(id, projectDto);
                    TempData["Success"] = "Proje başarıyla güncellendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating project");
                    ModelState.AddModelError("", "Proje güncellenirken hata oluştu.");
                }
            }
            return View(projectDto);
        }

        // POST: Project/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _projectService.DeleteProjectAsync(id);
                TempData["Success"] = "Proje başarıyla silindi!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project");
                TempData["Error"] = "Proje silinirken hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}