using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.Events.Where(e => !e.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Title.Contains(searchTerm) || e.Location.Contains(searchTerm) || e.ShortDescription.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(e => e.EventDate)
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
        public async Task<IActionResult> Create(Event ev, IFormFile? imageFile, List<IFormFile> galleryFiles)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        ev.ImagePath = await SaveFileAsync(imageFile);
                    }

                    var paths = new List<string>();
                    foreach (var file in galleryFiles)
                    {
                        if (file.Length > 0)
                        {
                            var path = await SaveFileAsync(file);
                            paths.Add(path);
                        }
                    }
                    ev.GalleryImagesJson = JsonSerializer.Serialize(paths);
                    ev.CreatedDate = DateTime.Now;
                    ev.UpdatedDate = DateTime.Now;

                    _context.Events.Add(ev);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to save event: {ex.Message}");
                }
            }
            return View(ev);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || ev.IsDeleted) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event ev, IFormFile? imageFile, List<IFormFile> galleryFiles)
        {
            if (id != ev.Id) return NotFound();

            var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (existingEvent == null || existingEvent.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    ev.ImagePath = existingEvent.ImagePath;
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        ev.ImagePath = await SaveFileAsync(imageFile);
                    }

                    var paths = new List<string>();
                    if (!string.IsNullOrEmpty(existingEvent.GalleryImagesJson))
                    {
                        paths = JsonSerializer.Deserialize<List<string>>(existingEvent.GalleryImagesJson) ?? new List<string>();
                    }

                    foreach (var file in galleryFiles)
                    {
                        if (file.Length > 0)
                        {
                            var path = await SaveFileAsync(file);
                            paths.Add(path);
                        }
                    }
                    ev.GalleryImagesJson = JsonSerializer.Serialize(paths);
                    ev.CreatedDate = existingEvent.CreatedDate;
                    ev.UpdatedDate = DateTime.Now;

                    _context.Update(ev);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Event updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update event: {ex.Message}");
                }
            }
            return View(ev);
        }

        public async Task<IActionResult> Details(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || ev.IsDeleted) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || ev.IsDeleted) return NotFound();

            ev.IsActive = !ev.IsActive;
            ev.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for event: {ev.Title}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || ev.IsDeleted) return NotFound();

            ev.IsDeleted = true;
            ev.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Event deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryImage(int id, string imagePath)
        {
            var ev = await _context.Events.FindAsync(id);
            if (ev == null || ev.IsDeleted) return NotFound();

            if (!string.IsNullOrEmpty(ev.GalleryImagesJson))
            {
                var paths = JsonSerializer.Deserialize<List<string>>(ev.GalleryImagesJson) ?? new List<string>();
                if (paths.Contains(imagePath))
                {
                    paths.Remove(imagePath);
                    ev.GalleryImagesJson = JsonSerializer.Serialize(paths);
                    ev.UpdatedDate = DateTime.Now;
                    await _context.SaveChangesAsync();

                    // Delete physical file
                    string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                    if (System.IO.File.Exists(physicalPath))
                    {
                        System.IO.File.Delete(physicalPath);
                    }
                }
            }
            return RedirectToAction(nameof(Edit), new { id = ev.Id });
        }

        private async Task<string> SaveFileAsync(IFormFile file)
        {
            string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string webPath = await gfps.Services.SafeFileUpload.SaveImageAsync(file, uploadPath);

            // Save to media files list
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
