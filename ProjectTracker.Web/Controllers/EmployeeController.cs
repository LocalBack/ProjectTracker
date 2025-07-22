using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Web.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace ProjectTracker.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // GET: Employee
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber, int? pageSize)
        {
            // Sorting parameters
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["TitleSortParm"] = sortOrder == "Title" ? "title_desc" : "Title";
            ViewData["HireDateSortParm"] = sortOrder == "HireDate" ? "hiredate_desc" : "HireDate";

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

            // Get all employees
            var employees = await _employeeService.GetAllEmployeesAsync();

            // Apply search filter
            if (!string.IsNullOrEmpty(searchString))
            {
                employees = employees.Where(e =>
                    e.FirstName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    e.LastName.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    e.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                    (e.Title != null && e.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Apply sorting
            employees = sortOrder switch
            {
                "name_desc" => employees.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName).ToList(),
                "Title" => employees.OrderBy(e => e.Title).ToList(),
                "title_desc" => employees.OrderByDescending(e => e.Title).ToList(),
                "HireDate" => employees.OrderBy(e => e.HireDate).ToList(),
                "hiredate_desc" => employees.OrderByDescending(e => e.HireDate).ToList(),
                _ => employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToList(),
            };

            // Pagination
            int currentPageSize = pageSize ?? 10;
            ViewData["CurrentPageSize"] = currentPageSize;

            var employeesList = employees.ToList();
            var count = employeesList.Count();

            // Create paginated list
            var paginatedEmployees = new PaginatedList<EmployeeDto>(
                employeesList.Skip(((pageNumber ?? 1) - 1) * currentPageSize).Take(currentPageSize).ToList(),
                count,
                pageNumber ?? 1,
                currentPageSize,
                searchString ?? "",
                sortOrder ?? ""
            );

            ViewData["TotalRecords"] = count;

            return View(paginatedEmployees);
        }

        // GET: Employee/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employee/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            var model = new EmployeeDto
            {
                HireDate = DateTime.Today
            };
            return View(model);
        }

        // POST: Employee/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Title,HireDate")] EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                await _employeeService.CreateEmployeeAsync(employeeDto);
                TempData["SuccessMessage"] = "Çalışan başarıyla oluşturuldu.";
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: Employee/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,Title,HireDate")] EmployeeDto employeeDto)
        {
            if (id != employeeDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _employeeService.UpdateEmployeeAsync(id, employeeDto);
                    TempData["SuccessMessage"] = "Çalışan başarıyla güncellendi.";
                }
                catch (Exception)
                {
                    TempData["ErrorMessage"] = "Çalışan güncellenirken bir hata oluştu.";
                    return View(employeeDto);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employeeDto);
        }

        // GET: Employee/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(id.Value);
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
            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                TempData["SuccessMessage"] = "Çalışan başarıyla silindi.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Çalışan silinirken bir hata oluştu.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}