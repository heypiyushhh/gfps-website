using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class AchievementsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AchievementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.Achievements.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || a.RecipientName.Contains(searchTerm) || a.Category.Contains(searchTerm) || a.Year.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.Year)
                                   .ThenByDescending(a => a.AchievementDate)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Achievement achievement, IFormFile? imageFile, IFormFile? certificateFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        achievement.ImagePath = await SaveFileAsync(imageFile, false);
                    }

                    if (certificateFile != null && certificateFile.Length > 0)
                    {
                        achievement.CertificatePath = await SaveFileAsync(certificateFile, true);
                    }

                    achievement.CreatedDate = DateTime.Now;
                    achievement.UpdatedDate = DateTime.Now;

                    _context.Achievements.Add(achievement);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Achievement created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to create achievement: {ex.Message}");
                }
            }
            return View(achievement);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null || achievement.IsDeleted) return NotFound();
            return View(achievement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Achievement achievement, IFormFile? imageFile, IFormFile? certificateFile)
        {
            if (id != achievement.Id) return NotFound();

            var existingAchievement = await _context.Achievements.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (existingAchievement == null || existingAchievement.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    achievement.ImagePath = existingAchievement.ImagePath;
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        achievement.ImagePath = await SaveFileAsync(imageFile, false);
                    }

                    achievement.CertificatePath = existingAchievement.CertificatePath;
                    if (certificateFile != null && certificateFile.Length > 0)
                    {
                        achievement.CertificatePath = await SaveFileAsync(certificateFile, true);
                    }

                    achievement.CreatedDate = existingAchievement.CreatedDate;
                    achievement.UpdatedDate = DateTime.Now;

                    _context.Update(achievement);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Achievement updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update achievement: {ex.Message}");
                }
            }
            return View(achievement);
        }

        public async Task<IActionResult> Details(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null || achievement.IsDeleted) return NotFound();
            return View(achievement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null || achievement.IsDeleted) return NotFound();

            achievement.IsActive = !achievement.IsActive;
            achievement.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for achievement: {achievement.Title}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var achievement = await _context.Achievements.FindAsync(id);
            if (achievement == null || achievement.IsDeleted) return NotFound();

            achievement.IsDeleted = true;
            achievement.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Achievement deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFileAsync(IFormFile file, bool isCertificate)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string webPath = isCertificate 
                ? await gfps.Services.SafeFileUpload.SaveCertificateAsync(file, uploadPath)
                : await gfps.Services.SafeFileUpload.SaveImageAsync(file, uploadPath);

            var media = new MediaFile
            {
                FileName = file.FileName,
                FilePath = webPath,
                SizeBytes = file.Length,
                ContentType = file.ContentType,
                UploadedAt = DateTime.Now,
                UploadedBy = User.Identity?.Name ?? "Admin"
            };
            _context.MediaFiles.Add(media);

            return webPath;
        }
    }
}
