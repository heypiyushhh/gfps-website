using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.News.Where(n => !n.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(n => n.Title.Contains(searchTerm) || n.Author.Contains(searchTerm) || n.Summary.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(n => n.PublishDate)
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
        public async Task<IActionResult> Create(News news, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        news.ImagePath = await SaveFileAsync(imageFile);
                    }

                    if (string.IsNullOrEmpty(news.Slug))
                    {
                        news.Slug = GenerateSlug(news.Title);
                    }
                    else
                    {
                        news.Slug = GenerateSlug(news.Slug);
                    }

                    news.CreatedDate = DateTime.Now;
                    news.UpdatedDate = DateTime.Now;

                    _context.News.Add(news);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "News article created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to save news: {ex.Message}");
                }
            }
            return View(news);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || news.IsDeleted) return NotFound();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, News news, IFormFile? imageFile)
        {
            if (id != news.Id) return NotFound();

            var existingNews = await _context.News.AsNoTracking().FirstOrDefaultAsync(n => n.Id == id);
            if (existingNews == null || existingNews.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    news.ImagePath = existingNews.ImagePath;
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        news.ImagePath = await SaveFileAsync(imageFile);
                    }

                    if (string.IsNullOrEmpty(news.Slug))
                    {
                        news.Slug = GenerateSlug(news.Title);
                    }
                    else
                    {
                        news.Slug = GenerateSlug(news.Slug);
                    }

                    news.CreatedDate = existingNews.CreatedDate;
                    news.UpdatedDate = DateTime.Now;

                    _context.Update(news);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "News article updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update news: {ex.Message}");
                }
            }
            return View(news);
        }

        public async Task<IActionResult> Details(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || news.IsDeleted) return NotFound();
            return View(news);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || news.IsDeleted) return NotFound();

            news.IsActive = !news.IsActive;
            news.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for article: {news.Title}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleFeatured(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || news.IsDeleted) return NotFound();

            news.IsFeatured = !news.IsFeatured;
            news.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Featured toggle updated for article: {news.Title}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null || news.IsDeleted) return NotFound();

            news.IsDeleted = true;
            news.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = "News article deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private string GenerateSlug(string title)
        {
            var str = title.ToLowerInvariant();
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"[^a-z0-9\-_]", "");
            str = Regex.Replace(str, @"\-{2,}", "-");
            return str.Trim('-');
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
