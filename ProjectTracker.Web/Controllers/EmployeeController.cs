using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;
using System.Security.Claims;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = "Employee,Manager,Admin")]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWorkLogService _workLogService;

        public EmployeeController(IEmployeeService employeeService, IWorkLogService workLogService)
        {
            _employeeService = employeeService;
            _workLogService = workLogService;
        }

        // GET: Employee/MyProfile
        public async Task<IActionResult> MyProfile()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return NotFound("User ID not found.");
            }

            // Try to parse the userId
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return NotFound("Invalid user ID format.");
            }

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);

            if (employee == null)
            {
                TempData["Warning"] = "Çalışan profili bulunamadı. Lütfen yöneticinizle iletişime geçin.";
                return RedirectToAction("Index", "Home");
            }

            return View(employee);
        }

        // GET: Employee/MyWorkLogs
        public async Task<IActionResult> MyWorkLogs()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return NotFound("User ID not found.");
            }

            // Try to parse the userId
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return NotFound("Invalid user ID format.");
            }

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
            if (employee == null)
            {
                TempData["Warning"] = "Çalışan profili bulunamadı.";
                return RedirectToAction("Index", "Home");
            }

            var workLogs = await _workLogService.GetWorkLogsByEmployeeIdAsync(employee.Id);
            return View(workLogs);
        }

        // GET: Employee (List all employees - Manager/Admin only)
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return View(employees);
        }

        // GET: Employee/Details/5
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // GET: Employee/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.CreateEmployeeAsync(employeeDto);
                TempData["Success"] = "Çalışan başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, EmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var result = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
                if (result == null)
                {
                    return NotFound();
                }
                TempData["Success"] = "Çalışan başarıyla güncellendi.";
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _employeeService.DeleteEmployeeAsync(id);
            if (!result)
            {
                return NotFound();
            }
            TempData["Success"] = "Çalışan başarıyla silindi.";
            return RedirectToAction(nameof(Index));
        }
    }
}