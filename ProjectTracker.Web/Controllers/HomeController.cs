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

        public HomeController(
            ILogger<HomeController> logger,
            IProjectService projectService,
            IWorkLogService workLogService)
        {
            _logger = logger;
            _projectService = projectService;
            _workLogService = workLogService;
        }

        [AllowAnonymous] // Ana sayfa herkese açýk
        public async Task<IActionResult> Index()
        {
            if (User.Identity?.IsAuthenticated ?? false)
            {
                var viewModel = new DashboardViewModel
                {
                    TotalProjects = await _projectService.GetProjectCountAsync(),
                    TotalEmployees = 8,
                    ActiveWorkLogs = 24,  // This is now int, not List
                    UpcomingMaintenances = 3,  // This is now int, not List
                    RecentWorkLogs = (await _workLogService.GetRecentWorkLogsAsync(5)).ToList()  // Add .ToList()
                };

                return View(viewModel);
            }

            return View();
        }

        // Privacy giriþ gerektiriyor (global policy'den)
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