using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Web.Controllers
{
    [Authorize]
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly ILogger<WorkLogController> _logger;

        public WorkLogController(
            IWorkLogService workLogService,
            IProjectService projectService,
            IEmployeeService employeeService,
            ILogger<WorkLogController> logger)
        {
            _workLogService = workLogService;
            _projectService = projectService;
            _employeeService = employeeService;
            _logger = logger;
        }

        // GET: WorkLog
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber,
            int? pageSize)
        {
            // ViewData ayarları
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["ProjectSortParm"] = sortOrder == "Project" ? "project_desc" : "Project";
            ViewData["EmployeeSortParm"] = sortOrder == "Employee" ? "employee_desc" : "Employee";

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

            // Tüm WorkLog'ları al
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            // IEnumerable'ı IQueryable'a çevir
            var queryableWorkLogs = workLogs.AsQueryable();

            // Filtreleme
            if (!String.IsNullOrEmpty(searchString))
            {
                queryableWorkLogs = queryableWorkLogs.Where(s =>
                    s.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.ProjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    s.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase));
            }

            // Sıralama
            switch (sortOrder)
            {
                case "date_desc":
                    queryableWorkLogs = queryableWorkLogs.OrderByDescending(s => s.WorkDate);
                    break;
                case "Project":
                    queryableWorkLogs = queryableWorkLogs.OrderBy(s => s.ProjectName);
                    break;
                case "project_desc":
                    queryableWorkLogs = queryableWorkLogs.OrderByDescending(s => s.ProjectName);
                    break;
                case "Employee":
                    queryableWorkLogs = queryableWorkLogs.OrderBy(s => s.EmployeeName);
                    break;
                case "employee_desc":
                    queryableWorkLogs = queryableWorkLogs.OrderByDescending(s => s.EmployeeName);
                    break;
                default:
                    queryableWorkLogs = queryableWorkLogs.OrderBy(s => s.WorkDate);
                    break;
            }

            // Sayfalama için önce listeye çevir
            var totalCount = queryableWorkLogs.Count();
            int selectedPageSize = pageSize ?? 10;
            var paginatedItems = queryableWorkLogs
                .Skip((pageNumber ?? 1 - 1) * selectedPageSize)
                .Take(selectedPageSize)
                .ToList();

            var paginatedList = new PaginatedList<WorkLogDto>(
                paginatedItems,
                totalCount,
                pageNumber ?? 1,
                selectedPageSize,
                searchString,
                sortOrder);

            return View(paginatedList);
        }

        // GET: WorkLog/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }

            return View(workLog);
        }

        // GET: WorkLog/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name");
            ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName");

            return View();
        }

        // POST: WorkLog/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkLogDto workLogDto)
        {
            if (ModelState.IsValid)
            {
                await _workLogService.CreateWorkLogAsync(workLogDto);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLogDto.ProjectId);
            ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLogDto.EmployeeId);

            return View(workLogDto);
        }

        // GET: WorkLog/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLog.ProjectId);
            ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLog.EmployeeId);

            return View(workLog);
        }

        // POST: WorkLog/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkLogDto workLogDto)
        {
            if (id != workLogDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _workLogService.UpdateWorkLogAsync(id, workLogDto);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLogDto.ProjectId);
            ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLogDto.EmployeeId);

            return View(workLogDto);
        }

        // GET: WorkLog/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            if (workLog == null)
            {
                return NotFound();
            }

            return View(workLog);
        }

        // POST: WorkLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _workLogService.DeleteWorkLogAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}