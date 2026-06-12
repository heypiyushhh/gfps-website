using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using gfps.Data;
using gfps.Models;
using System.Threading.Tasks;
using System;

namespace gfps.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password, bool rememberMe, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email and password are required.");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                if (!user.IsActive)
                {
                    ModelState.AddModelError("", "Your account has been deactivated. Please contact the administrator.");
                    await RecordLoginAttemptAsync(email, user.Id, false);
                    return View();
                }

                var result = await _signInManager.PasswordSignInAsync(user, password, rememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    user.LastLoginDate = DateTime.Now;
                    await _userManager.UpdateAsync(user);
                    await RecordLoginAttemptAsync(email, user.Id, true);

                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                }
                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("", "This account is locked due to too many failed login attempts.");
                    await RecordLoginAttemptAsync(email, user.Id, false);
                    return View();
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            await RecordLoginAttemptAsync(email, user?.Id ?? "Unknown", false);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        private async Task RecordLoginAttemptAsync(string email, string userId, bool isSuccess)
        {
            var history = new LoginHistory
            {
                UserId = userId,
                UserEmail = email,
                LoginTime = DateTime.Now,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                BrowserAgent = Request.Headers["User-Agent"].ToString(),
                IsSuccessful = isSuccess
            };

            _context.LoginHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}
