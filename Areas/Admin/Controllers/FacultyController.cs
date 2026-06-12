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
    public class FacultyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacultyController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var faculty = await _context.FacultyMembers.OrderBy(f => f.DisplayOrder).ToListAsync();
            return View(faculty);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FacultyMember faculty, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    string webPath = await gfps.Services.SafeFileUpload.SaveImageAsync(imageFile, uploadPath);
                    faculty.ImagePath = webPath;

                    // Add to Media Library
                    var media = new MediaFile
                    {
                        FileName = imageFile.FileName,
                        FilePath = webPath,
                        SizeBytes = imageFile.Length,
                        ContentType = imageFile.ContentType,
                        UploadedAt = DateTime.Now,
                        UploadedBy = User.Identity?.Name ?? "Admin"
                    };
                    _context.MediaFiles.Add(media);
                }

                _context.FacultyMembers.Add(faculty);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var faculty = await _context.FacultyMembers.FindAsync(id);
            if (faculty == null) return NotFound();
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FacultyMember faculty, IFormFile? imageFile)
        {
            if (id != faculty.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        string uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                        string webPath = await gfps.Services.SafeFileUpload.SaveImageAsync(imageFile, uploadPath);
                        faculty.ImagePath = webPath;

                        // Add to Media Library
                        var media = new MediaFile
                        {
                            FileName = imageFile.FileName,
                            FilePath = webPath,
                            SizeBytes = imageFile.Length,
                            ContentType = imageFile.ContentType,
                            UploadedAt = DateTime.Now,
                            UploadedBy = User.Identity?.Name ?? "Admin"
                        };
                        _context.MediaFiles.Add(media);
                    }

                    _context.Update(faculty);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyExists(faculty.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(faculty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var faculty = await _context.FacultyMembers.FindAsync(id);
            if (faculty != null)
            {
                _context.FacultyMembers.Remove(faculty);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Faculty member deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool FacultyExists(int id)
        {
            return _context.FacultyMembers.Any(e => e.Id == id);
        }
    }
}
