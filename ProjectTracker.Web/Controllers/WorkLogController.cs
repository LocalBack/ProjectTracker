using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.Controllers
{
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IProjectService _projectService;
        private readonly ILogger<WorkLogController> _logger;

        public WorkLogController(
            IWorkLogService workLogService,
            IProjectService projectService,
            ILogger<WorkLogController> logger)
        {
            _workLogService = workLogService;
            _projectService = projectService;
            _logger = logger;
        }

        // GET: WorkLog
        public async Task<IActionResult> Index(int? projectId, int? employeeId)
        {
            try
            {
                IEnumerable<WorkLogDto> workLogs;

                if (projectId.HasValue)
                {
                    workLogs = await _workLogService.GetWorkLogsByProjectIdAsync(projectId.Value);
                    ViewBag.FilterType = "Project";
                    var project = await _projectService.GetProjectByIdAsync(projectId.Value);
                    ViewBag.FilterName = project?.Name;
                }
                else if (employeeId.HasValue)
                {
                    workLogs = await _workLogService.GetWorkLogsByEmployeeIdAsync(employeeId.Value);
                    ViewBag.FilterType = "Employee";
                    // Employee name için service eklenecek
                }
                else
                {
                    workLogs = await _workLogService.GetAllWorkLogsAsync();
                }

                return View(workLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting work logs");
                TempData["Error"] = "İş kayıtları yüklenirken hata oluştu.";
                return View(new List<WorkLogDto>());
            }
        }

        // GET: WorkLog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }

            ViewBag.Details = await _workLogService.GetWorkLogDetailsAsync(id);
            ViewBag.Attachments = await _workLogService.GetWorkLogAttachmentsAsync(id);

            return View(workLog);
        }

        // GET: WorkLog/Create
        public async Task<IActionResult> Create(int? projectId)
        {
            await LoadDropdowns();

            var model = new CreateWorkLogDto
            {
                WorkDate = DateTime.Today,
                ProjectId = projectId ?? 0
            };

            return View(model);
        }

        // POST: WorkLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWorkLogDto workLogDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Şimdilik EmployeeId'yi 1 yapıyoruz (sonra login olan kullanıcıdan alınacak)
                    workLogDto.EmployeeId = 1;

                    await _workLogService.CreateWorkLogAsync(workLogDto);
                    TempData["Success"] = "İş kaydı başarıyla oluşturuldu!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating work log");
                    ModelState.AddModelError("", "İş kaydı oluşturulurken hata oluştu.");
                }
            }

            await LoadDropdowns();
            return View(workLogDto);
        }

        private async Task LoadDropdowns()
        {
            var projects = await _projectService.GetActiveProjectsAsync();
            ViewBag.Projects = new SelectList(projects, "Id", "Name");
        }
    }
}