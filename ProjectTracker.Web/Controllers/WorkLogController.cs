using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using System.Security.Claims;
using ProjectTracker.Web.Authorization;
using System.IO;
using System;

using System.Collections.Generic;
using System.Linq;


namespace ProjectTracker.Web.Controllers
{
    [Authorize]
    public class WorkLogController : Controller
    {
        private readonly IWorkLogService _workLogService;
        private readonly IProjectService _projectService;
        private readonly IEmployeeService _employeeService;
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<WorkLogController> _logger;

        private static readonly string[] _allowedExtensions = new[] { ".pdf", ".png" };
        private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

        public WorkLogController(
            IWorkLogService workLogService,
            IProjectService projectService,
            IEmployeeService employeeService,
            IAuthorizationService authorizationService,
            ILogger<WorkLogController> logger)
        {
            _workLogService = workLogService;
            _projectService = projectService;
            _employeeService = employeeService;
            _authorizationService = authorizationService;
            _logger = logger;
        }

        // GET: WorkLog
        [Authorize(Roles = "Admin,Manager,Employee")]
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

            // Get work logs based on user role
            IEnumerable<WorkLogDto> workLogs;
            var userIdValue = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (User.IsInRole("Admin") || User.IsInRole("Manager"))
            {
                workLogs = await _workLogService.GetAllWorkLogsAsync();
            }
            else if (User.IsInRole("Employee"))
            {
                if (string.IsNullOrEmpty(userIdValue) || !int.TryParse(userIdValue, out int userId))
                {
                    return Unauthorized();
                }
                workLogs = await _workLogService.GetWorkLogsByUserIdAsync(userId);
            }
            else
            {
                return Forbid();
            }

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
        [Authorize(Roles = "Admin,Manager,Employee")]
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
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Details(int id)
        {
            var workLogEntity = await _workLogService.GetWorkLogEntityByIdAsync(id);
            if (workLogEntity == null)
            {
                return NotFound();
            }


            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workLogEntity, new WorkLogOwnerRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            ViewBag.Details = workLog?.Details;
            ViewBag.Attachments = workLog?.Attachments;

            return View(workLog);
        }

        // GET: WorkLog/Create
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name");

            var model = new WorkLogDto();

            if (User.IsInRole("Employee"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                {
                    return Forbid();
                }

                model.EmployeeId = employee.Id;
                ViewData["EmployeeId"] = new SelectList(new[] { employee }, "Id", "FullName", employee.Id);
            }
            else
            {
                ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName");
            }

            return View(model);
        }

        // POST: WorkLog/Create
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WorkLogDto workLogDto, IFormFile? attachment)
        {
            if (User.IsInRole("Employee"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }

                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                if (employee == null)
                {
                    return Forbid();
                }

                // Force the work log to use the current employee's id to prevent tampering
                workLogDto.EmployeeId = employee.Id;
            }

            if (attachment != null && attachment.Length > 0)
            {
                var ext = Path.GetExtension(attachment.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Attachment", "Unsupported file type. Only PDF and PNG are allowed.");
                }
                else if (attachment.Length > _maxFileSize)
                {
                    ModelState.AddModelError("Attachment", "File size exceeds the 5 MB limit.");
                }
            }

            if (ModelState.IsValid)
            {
                if (attachment != null && attachment.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(attachment.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }

                    workLogDto.Attachments.Add(new WorkLogAttachmentDto
                    {
                        FileName = fileName,
                        FilePath = Path.Combine("uploads", uniqueFileName),
                        FileType = attachment.ContentType,
                        FileSize = attachment.Length
                    });
                }

                await _workLogService.CreateWorkLogAsync(workLogDto);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLogDto.ProjectId);
            if (User.IsInRole("Employee"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                    ViewData["EmployeeId"] = new SelectList(new[] { employee }, "Id", "FullName", employee?.Id);
                    workLogDto.EmployeeId = employee?.Id ?? workLogDto.EmployeeId;
                }
            }
            else
            {
                ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLogDto.EmployeeId);
            }

            return View(workLogDto);
        }

        // GET: WorkLog/Edit/5
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Edit(int id)
        {
            var workLogEntity = await _workLogService.GetWorkLogEntityByIdAsync(id);
            if (workLogEntity == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workLogEntity, new WorkLogOwnerRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var workLog = await _workLogService.GetWorkLogByIdAsync(id);
            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLog.ProjectId);
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized();
                }
                var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                if (employee == null || workLog.EmployeeId != employee.Id)
                {
                    return Forbid();
                }
                ViewData["EmployeeId"] = new SelectList(new[] { employee }, "Id", "FullName", workLog.EmployeeId);
            }
            else
            {
                ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLog.EmployeeId);
            }

            return View(workLog);
        }

        // POST: WorkLog/Edit/5
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WorkLogDto workLogDto, IFormFile? attachment)
        {
            if (id != workLogDto.Id)
            {
                return NotFound();
            }


            var workLogEntity = await _workLogService.GetWorkLogEntityByIdAsync(id);
            if (workLogEntity == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workLogEntity, new WorkLogOwnerRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();

            }

            if (attachment != null && attachment.Length > 0)
            {
                var ext = Path.GetExtension(attachment.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(ext))
                {
                    ModelState.AddModelError("Attachment", "Unsupported file type. Only PDF and PNG are allowed.");
                }
                else if (attachment.Length > _maxFileSize)
                {
                    ModelState.AddModelError("Attachment", "File size exceeds the 5 MB limit.");
                }
            }

            if (ModelState.IsValid)
            {
                var existing = await _workLogService.GetWorkLogByIdAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                workLogDto.Attachments = existing.Attachments?.ToList() ?? new List<WorkLogAttachmentDto>();

                if (attachment != null && attachment.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Path.GetFileName(attachment.FileName);
                    var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await attachment.CopyToAsync(stream);
                    }

                    workLogDto.Attachments.Add(new WorkLogAttachmentDto
                    {
                        FileName = fileName,
                        FilePath = Path.Combine("uploads", uniqueFileName),
                        FileType = attachment.ContentType,
                        FileSize = attachment.Length
                    });
                }

                await _workLogService.UpdateWorkLogAsync(id, workLogDto);
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectId"] = new SelectList(await _projectService.GetAllProjectsAsync(), "Id", "Name", workLogDto.ProjectId);
            if (!User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int userId))
                {
                    var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
                    ViewData["EmployeeId"] = new SelectList(new[] { employee }, "Id", "FullName", workLogDto.EmployeeId);
                }
            }
            else
            {
                ViewData["EmployeeId"] = new SelectList(await _employeeService.GetAllEmployeesAsync(), "Id", "FullName", workLogDto.EmployeeId);
            }

            return View(workLogDto);
        }

        // GET: WorkLog/Delete/5
        [Authorize(Roles = "Admin,Manager,Employee")]
        public async Task<IActionResult> Delete(int id)
        {
            var workLogEntity = await _workLogService.GetWorkLogEntityByIdAsync(id);
            if (workLogEntity == null)
            {
                return NotFound();
            }


            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workLogEntity, new WorkLogOwnerRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            var workLog = await _workLogService.GetWorkLogByIdAsync(id);

            return View(workLog);
        }

        // POST: WorkLog/Delete/5
        [Authorize(Roles = "Admin,Manager,Employee")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var workLogEntity = await _workLogService.GetWorkLogEntityByIdAsync(id);
            if (workLogEntity == null)

            {
                return NotFound();
            }


            var authorizationResult = await _authorizationService.AuthorizeAsync(User, workLogEntity, new WorkLogOwnerRequirement());
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

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