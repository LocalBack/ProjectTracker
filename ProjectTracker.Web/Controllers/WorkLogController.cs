using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using System.Security.Claims;

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
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            // Sorting parameters
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["ProjectSortParm"] = sortOrder == "Project" ? "project_desc" : "Project";
            ViewData["EmployeeSortParm"] = sortOrder == "Employee" ? "employee_desc" : "Employee";
            ViewData["HoursSortParm"] = sortOrder == "Hours" ? "hours_desc" : "Hours";

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

            // Get all work logs
            var workLogs = await _workLogService.GetAllWorkLogsAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                workLogs = workLogs.Where(w =>
                    w.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    w.ProjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    w.EmployeeName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (w.Description != null && w.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply sorting
            workLogs = sortOrder switch
            {
                "date_desc" => workLogs.OrderByDescending(w => w.WorkDate).ToList(),
                "Title" => workLogs.OrderBy(w => w.Title).ToList(),
                "title_desc" => workLogs.OrderByDescending(w => w.Title).ToList(),
                "Project" => workLogs.OrderBy(w => w.ProjectName).ToList(),
                "project_desc" => workLogs.OrderByDescending(w => w.ProjectName).ToList(),
                "Employee" => workLogs.OrderBy(w => w.EmployeeName).ToList(),
                "employee_desc" => workLogs.OrderByDescending(w => w.EmployeeName).ToList(),
                "Hours" => workLogs.OrderBy(w => w.HoursSpent).ToList(),
                "hours_desc" => workLogs.OrderByDescending(w => w.HoursSpent).ToList(),
                _ => workLogs.OrderBy(w => w.WorkDate).ToList(),
            };

            // Pagination
            int currentPageSize = pageSize ?? 10;
            ViewData["CurrentPageSize"] = currentPageSize;

            var workLogsList = workLogs.ToList();
            var count = workLogsList.Count();

            // Create paginated list
            var paginatedWorkLogs = new PaginatedList<WorkLogDto>(
                workLogsList.Skip(((pageNumber ?? 1) - 1) * currentPageSize).Take(currentPageSize).ToList(),
                count,
                pageNumber ?? 1,
                currentPageSize,
                searchString ?? "",
                sortOrder ?? ""
            );

            // Calculate total hours
            ViewData["TotalHours"] = workLogsList.Sum(w => w.HoursSpent);
            ViewData["TotalRecords"] = count;

            return View(paginatedWorkLogs);
        }

        // GET: WorkLog/MyWorkLog
        public async Task<IActionResult> MyWorkLog(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            // Get current user's work logs
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
            {
                return RedirectToAction("Login", "Account");
            }

            // Similar logic as Index but filtered by user
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = string.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["ProjectSortParm"] = sortOrder == "Project" ? "project_desc" : "Project";
            ViewData["HoursSortParm"] = sortOrder == "Hours" ? "hours_desc" : "Hours";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var workLogs = await _workLogService.GetWorkLogsByUserIdAsync(userIdInt);

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                workLogs = workLogs.Where(w =>
                    w.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    w.ProjectName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (w.Description != null && w.Description.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply sorting
            workLogs = sortOrder switch
            {
                "date_desc" => workLogs.OrderByDescending(w => w.WorkDate).ToList(),
                "Title" => workLogs.OrderBy(w => w.Title).ToList(),
                "title_desc" => workLogs.OrderByDescending(w => w.Title).ToList(),
                "Project" => workLogs.OrderBy(w => w.ProjectName).ToList(),
                "project_desc" => workLogs.OrderByDescending(w => w.ProjectName).ToList(),
                "Hours" => workLogs.OrderBy(w => w.HoursSpent).ToList(),
                "hours_desc" => workLogs.OrderByDescending(w => w.HoursSpent).ToList(),
                _ => workLogs.OrderBy(w => w.WorkDate).ToList(),
            };

            // Pagination
            int currentPageSize = pageSize ?? 10;
            ViewData["CurrentPageSize"] = currentPageSize;

            var workLogsList = workLogs.ToList();
            var count = workLogsList.Count();

            var paginatedWorkLogs = new PaginatedList<WorkLogDto>(
                workLogsList.Skip(((pageNumber ?? 1) - 1) * currentPageSize).Take(currentPageSize).ToList(),
                count,
                pageNumber ?? 1,
                currentPageSize,
                searchString ?? "",
                sortOrder ?? ""
            );

            ViewData["TotalHours"] = workLogsList.Sum(w => w.HoursSpent);
            ViewData["TotalRecords"] = count;

            return View(paginatedWorkLogs);
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
            public async Task<IActionResult> MyWorkLogs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return RedirectToAction("Index", "Home");
            }

            // Get employee by user ID
            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
            if (employee == null)
            {
                TempData["Warning"] = "Çalışan profili bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var workLogs = await _workLogService.GetWorkLogsByEmployeeIdAsync(employee.Id);
            return View(workLogs);
            }
        }
    }