using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Web.Models;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;

namespace ProjectTracker.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProjectService _projectService;
        private readonly IWorkLogService _workLogService;
        private readonly IEmployeeService _employeeService;
        private readonly IMaintenanceScheduleService _maintenanceScheduleService;

        public HomeController(
            ILogger<HomeController> logger,
            IProjectService projectService,
            IWorkLogService workLogService,
            IEmployeeService employeeService,
            IMaintenanceScheduleService maintenanceScheduleService)
        {
            _logger = logger;
            _projectService = projectService;
            _workLogService = workLogService;
            _employeeService = employeeService;
            _maintenanceScheduleService = maintenanceScheduleService;
        }

        [AllowAnonymous] // Ana sayfa herkese açık
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var viewModel = new DashboardViewModel
                {
                    TotalProjects = await _projectService.GetProjectCountAsync(),
                    TotalEmployees = await _employeeService.GetEmployeeCountAsync(),
                    ActiveWorkLogs = await _workLogService.GetActiveWorkLogCountAsync(),
                    UpcomingMaintenances = await _maintenanceScheduleService.GetUpcomingMaintenanceCountAsync(),
                    RecentWorkLogs = (await _workLogService.GetRecentWorkLogsAsync(5)).ToList()
                };

                return View(viewModel);
            }

            return View();
        }

        // Privacy giriş gerektiriyor (global policy'den)
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}