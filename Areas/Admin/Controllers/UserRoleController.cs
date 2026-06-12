using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin")]
    public class UserRoleController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public UserRoleController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = string.Join(", ", roles);
            }

            ViewBag.UserRoles = userRoles;
            ViewBag.AuditLogs = await _context.AuditLogs.OrderByDescending(l => l.Timestamp).Take(20).ToListAsync();
            ViewBag.LoginHistory = await _context.LoginHistories.OrderByDescending(h => h.LoginTime).Take(20).ToListAsync();

            return View(users);
        }

        public IActionResult Create()
        {
            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string email, string fullName, string password, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
            {
                ModelState.AddModelError("", "All fields are required.");
                ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FullName = fullName,
                IsActive = true,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                TempData["Success"] = $"Admin user '{fullName}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            ViewBag.Roles = _roleManager.Roles.Select(r => r.Name).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                // Prevent self-lockout
                if (user.UserName == User.Identity?.Name)
                {
                    TempData["Error"] = "You cannot deactivate your own account.";
                    return RedirectToAction(nameof(Index));
                }

                user.IsActive = !user.IsActive;
                await _userManager.UpdateAsync(user);
                TempData["Success"] = $"User '{user.FullName}' status updated.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AuditLogs()
        {
            var logs = await _context.AuditLogs.OrderByDescending(l => l.Timestamp).ToListAsync();
            return View(logs);
        }

        public async Task<IActionResult> LoginHistory()
        {
            var history = await _context.LoginHistories.OrderByDescending(h => h.LoginTime).ToListAsync();
            return View(history);
        }
    }
}
