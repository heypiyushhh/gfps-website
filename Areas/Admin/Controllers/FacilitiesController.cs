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
    public class FacilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.Facilities.Where(f => !f.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.Name.Contains(searchTerm) || f.Description.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderBy(f => f.DisplayOrder)
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
        public async Task<IActionResult> Create(Facility facility, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        facility.ImagePath = await SaveFileAsync(imageFile);
                    }

                    facility.CreatedDate = DateTime.Now;
                    facility.UpdatedDate = DateTime.Now;

                    _context.Facilities.Add(facility);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Facility created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to create facility: {ex.Message}");
                }
            }
            return View(facility);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null || facility.IsDeleted) return NotFound();
            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Facility facility, IFormFile? imageFile)
        {
            if (id != facility.Id) return NotFound();

            var existingFacility = await _context.Facilities.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id);
            if (existingFacility == null || existingFacility.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    facility.ImagePath = existingFacility.ImagePath;
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        facility.ImagePath = await SaveFileAsync(imageFile);
                    }

                    facility.CreatedDate = existingFacility.CreatedDate;
                    facility.UpdatedDate = DateTime.Now;

                    _context.Update(facility);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Facility updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update facility: {ex.Message}");
                }
            }
            return View(facility);
        }

        public async Task<IActionResult> Details(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null || facility.IsDeleted) return NotFound();
            return View(facility);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null || facility.IsDeleted) return NotFound();

            facility.IsActive = !facility.IsActive;
            facility.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for facility: {facility.Name}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var facility = await _context.Facilities.FindAsync(id);
            if (facility == null || facility.IsDeleted) return NotFound();

            facility.IsDeleted = true;
            facility.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Facility deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string webPath = await gfps.Services.SafeFileUpload.SaveImageAsync(file, uploadPath);

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
