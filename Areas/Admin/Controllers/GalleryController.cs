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
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager")]
    public class GalleryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GalleryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm, int page = 1, int pageSize = 5)
        {
            var query = _context.GalleryAlbums.Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Name.Contains(searchTerm) || a.EventTag.Contains(searchTerm));
            }

            var totalItems = await query.CountAsync();
            var items = await query.OrderByDescending(a => a.CreatedDate)
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
        public async Task<IActionResult> Create(GalleryAlbum album, IFormFile? coverImageFile)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (coverImageFile != null && coverImageFile.Length > 0)
                    {
                        album.CoverImageUrl = await SaveFileAsync(coverImageFile);
                    }

                    album.CreatedDate = DateTime.Now;
                    album.UpdatedDate = DateTime.Now;

                    _context.GalleryAlbums.Add(album);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Album created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to create album: {ex.Message}");
                }
            }
            return View(album);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var album = await _context.GalleryAlbums.FindAsync(id);
            if (album == null || album.IsDeleted) return NotFound();
            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GalleryAlbum album, IFormFile? coverImageFile)
        {
            if (id != album.Id) return NotFound();

            var existingAlbum = await _context.GalleryAlbums.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (existingAlbum == null || existingAlbum.IsDeleted) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    album.CoverImageUrl = existingAlbum.CoverImageUrl;
                    if (coverImageFile != null && coverImageFile.Length > 0)
                    {
                        album.CoverImageUrl = await SaveFileAsync(coverImageFile);
                    }

                    album.CreatedDate = existingAlbum.CreatedDate;
                    album.UpdatedDate = DateTime.Now;

                    _context.Update(album);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Album updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Failed to update album: {ex.Message}");
                }
            }
            return View(album);
        }

        public async Task<IActionResult> Details(int id)
        {
            var album = await _context.GalleryAlbums
                .Include(a => a.GalleryItems)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null || album.IsDeleted) return NotFound();

            // Filter out soft deleted items
            ViewBag.GalleryItems = album.GalleryItems.Where(i => !i.IsDeleted).ToList();

            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var album = await _context.GalleryAlbums.FindAsync(id);
            if (album == null || album.IsDeleted) return NotFound();

            album.IsActive = !album.IsActive;
            album.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Status updated for album: {album.Name}";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _context.GalleryAlbums
                .Include(a => a.GalleryItems)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null || album.IsDeleted) return NotFound();

            album.IsDeleted = true;
            album.UpdatedDate = DateTime.Now;

            foreach (var item in album.GalleryItems)
            {
                item.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Album and all its items deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages(int albumId, List<IFormFile> files)
        {
            var album = await _context.GalleryAlbums.FindAsync(albumId);
            if (album == null || album.IsDeleted) return NotFound();

            if (files == null || files.Count == 0)
            {
                return Json(new { success = false, message = "No files selected." });
            }

            int successCount = 0;
            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    try
                    {
                        var path = await SaveFileAsync(file);
                        var item = new GalleryItem
                        {
                            Title = Path.GetFileNameWithoutExtension(file.FileName),
                            Category = album.EventTag,
                            FilePath = path,
                            IsVideo = false,
                            GalleryAlbumId = album.Id
                        };
                        _context.GalleryItems.Add(item);
                        successCount++;
                    }
                    catch
                    {
                        // Skip failed file
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, message = $"Uploaded {successCount} images successfully." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id, int albumId)
        {
            var item = await _context.GalleryItems.FindAsync(id);
            if (item == null || item.IsDeleted) return NotFound();

            item.IsDeleted = true;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Image removed from album.";
            return RedirectToAction(nameof(Details), new { id = albumId });
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
