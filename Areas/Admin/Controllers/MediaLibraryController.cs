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
    public class MediaLibraryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MediaLibraryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var files = await _context.MediaFiles.OrderByDescending(f => f.UploadedAt).ToListAsync();
            return View(files);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "No file selected.";
                return RedirectToAction(nameof(Index));
            }

            try
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
                await _context.SaveChangesAsync();
                TempData["Success"] = "File uploaded successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"File upload failed: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var media = await _context.MediaFiles.FindAsync(id);
            if (media != null)
            {
                // Delete physical file
                string physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", media.FilePath.TrimStart('/'));
                if (System.IO.File.Exists(physicalPath))
                {
                    System.IO.File.Delete(physicalPath);
                }

                _context.MediaFiles.Remove(media);
                await _context.SaveChangesAsync();
                TempData["Success"] = "File deleted from media library.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
