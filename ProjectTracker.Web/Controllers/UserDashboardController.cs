using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProjectTracker.Web.Controllers
{
    [Authorize(Roles = "User,Employee,Manager,Admin")] // Basic User role and above
    public class UserDashboardController : Controller
    {
        // Any authenticated user with at least "User" role can access
        public IActionResult Index()
        {
            ViewBag.UserName = User.Identity.Name;
            ViewBag.Roles = User.FindAll(System.Security.Claims.ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return View();
        }

        // User-specific actions
        public IActionResult MySettings()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }
    }
}