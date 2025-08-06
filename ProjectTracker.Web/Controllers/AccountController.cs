using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Core.Entities;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace ProjectTracker.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmployeeService _employeeService;
        private readonly IUserProjectSyncService _userProjectSyncService;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            IEmployeeService employeeService,
            IUserProjectSyncService userProjectSyncService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _employeeService = employeeService;
            _userProjectSyncService = userProjectSyncService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                _logger.LogInformation($"User {model.Email} logged in.");
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);
                }
                return RedirectToLocal(returnUrl);
            }

            if (result.RequiresTwoFactor)
                return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });

            if (result.IsLockedOut)
                return View("Lockout");

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated ?? false)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!model.KvkkAccepted)
            {
                ModelState.AddModelError(nameof(model.KvkkAccepted), "KVKK onam formunu onaylamalısınız.");
            }

            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedDate = DateTime.UtcNow,
                Kvkk = model.KvkkAccepted,
                KvkkTimestamp = model.KvkkAccepted ? DateTime.UtcNow : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            _logger.LogInformation($"User {model.Email} created a new account.");
            await _userManager.AddToRoleAsync(user, "User");
            await _signInManager.SignInAsync(user, isPersistent: false);

            // Sync UserProjects
            var employeeDto = await _employeeService.GetEmployeeByEmailAsync(user.Email);
            if (employeeDto != null)
            {
                await _userProjectSyncService.EnsureUserProjectsSyncedAsync(user.Id);
                _logger.LogInformation($"UserProjects synchronized for user {user.Email}.");
            }

            return RedirectToLocal(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string? returnUrl = null)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
                throw new InvalidOperationException("Unable to load two-factor authentication user.");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginWith2faViewModel { RememberMe = rememberMe });
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        [AllowAnonymous]
        public IActionResult Lockout() => View();

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                CreatedDate = user.CreatedDate,
                LastLoginDate = user.LastLoginDate
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var changePwd = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!changePwd.Succeeded)
            {
                foreach (var error in changePwd.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            await _signInManager.RefreshSignInAsync(user);
            _logger.LogInformation("User changed their password successfully.");
            return RedirectToAction(nameof(Profile));
        }

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}
