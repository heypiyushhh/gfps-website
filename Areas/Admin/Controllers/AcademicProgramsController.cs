using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class AcademicProgramsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AcademicProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.AcademicPrograms.Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm) || p.Department.Contains(searchTerm) || p.Subject.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderBy(p => p.DisplayOrder)
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AcademicProgram program, string curriculumText, IFormFile? featuredImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (featuredImageFile != null && featuredImageFile.Length > 0)
                    {
                        program.FeaturedImage = await SaveFileAsync(featuredImageFile);
                    }

                    var list = ParseCurriculum(curriculumText);
                    program.CurriculumJson = JsonSerializer.Serialize(list);
                    program.CreatedDate = DateTime.Now;
                    program.UpdatedDate = DateTime.Now;

                    _context.AcademicPrograms.Add(program);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Academic program created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to save program: {ex.Message}");
                }
            }
            return View(program);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);
            if (program == null || program.IsDeleted) return NotFound();

            try
            {
                var list = JsonSerializer.Deserialize<List<string>>(program.CurriculumJson) ?? new List<string>();
                ViewBag.CurriculumText = string.Join(Environment.NewLine, list);
            }
            catch
            {
                ViewBag.CurriculumText = string.Empty;
            }

            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AcademicProgram program, string curriculumText, IFormFile? featuredImageFile)
        {
            if (id != program.Id) return NotFound();

            var existingProgram = await _context.AcademicPrograms.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (existingProgram == null || existingProgram.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    program.FeaturedImage = existingProgram.FeaturedImage;
                    if (featuredImageFile != null && featuredImageFile.Length > 0)
                    {
                        program.FeaturedImage = await SaveFileAsync(featuredImageFile);
                    }

                    var list = ParseCurriculum(curriculumText);
                    program.CurriculumJson = JsonSerializer.Serialize(list);
                    program.CreatedDate = existingProgram.CreatedDate;
                    program.UpdatedDate = DateTime.Now;

                    _context.Update(program);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Academic program updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update program: {ex.Message}");
                }
            }

            return View(program);
        }

        public async Task<IActionResult> Details(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);
            if (program == null || program.IsDeleted) return NotFound();

            try
            {
                ViewBag.CurriculumList = JsonSerializer.Deserialize<List<string>>(program.CurriculumJson) ?? new List<string>();
            }
            catch
            {
                ViewBag.CurriculumList = new List<string>();
            }

            return View(program);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);
            if (program == null || program.IsDeleted) return NotFound();

            program.IsActive = !program.IsActive;
            program.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for program: {program.Title}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var program = await _context.AcademicPrograms.FindAsync(id);
            if (program == null || program.IsDeleted) return NotFound();

            program.IsDeleted = true;
            program.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Academic program deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private List<string> ParseCurriculum(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new List<string>();
            return text.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries)
                       .Select(line => line.Trim())
                       .Where(line => !string.IsNullOrEmpty(line))
                       .ToList();
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
