using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

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
        [Authorize(Policy = "CanViewReports")]
        public async Task<IActionResult> Index()
        {
            var projects = await _projectService.GetAllProjectsAsync();
            return View(projects);
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
        [Authorize(Policy = "CanManageProjects")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "CanManageProjects")]
        public async Task<IActionResult> Create(ProjectDto projectDto)
        {
            if (ModelState.IsValid)
            {
                await _projectService.CreateProjectAsync(projectDto);
                TempData["Success"] = "Proje başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Edit/5
        [Authorize(Policy = "CanManageProjects")]
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
        [Authorize(Policy = "CanManageProjects")]
        public async Task<IActionResult> Edit(int id, ProjectDto projectDto)
        {
            if (id != projectDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _projectService.UpdateProjectAsync(id, projectDto);
                if (result == null)
                {
                    return NotFound();
                }
                TempData["Success"] = "Proje başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(projectDto);
        }

        // GET: Project/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _projectService.DeleteProjectAsync(id);
            if (!result)
            {
                return NotFound();
            }
            TempData["Success"] = "Proje başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}