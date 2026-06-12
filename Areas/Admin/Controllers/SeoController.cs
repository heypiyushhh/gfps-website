using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SeoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SeoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var seoList = await _context.SeoMetadata.ToListAsync();
            return View(seoList);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var seo = await _context.SeoMetadata.FindAsync(id);
            if (seo == null) return NotFound();
            return View(seo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SeoMetadata seo)
        {
            if (id != seo.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(seo);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"SEO settings for '{seo.PageName}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(seo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RebuildSitemap()
        {
            try
            {
                string sitemapPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "sitemap.xml");
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                sb.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

                // Get base URL (or assume relative/root paths depending on hosting)
                string host = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host;

                // Add homepage
                sb.AppendLine("  <url>");
                sb.AppendLine($"    <loc>{host}/</loc>");
                sb.AppendLine($"    <lastmod>{DateTime.Now:yyyy-MM-dd}</lastmod>");
                sb.AppendLine("    <changefreq>daily</changefreq>");
                sb.AppendLine("    <priority>1.0</priority>");
                sb.AppendLine("  </url>");

                // Add other pages from SEO table
                var pages = await _context.SeoMetadata.Where(p => p.PageName != "Home").ToListAsync();
                foreach (var page in pages)
                {
                    string path = page.PageName.ToLower() switch
                    {
                        "about" => "/Home/About",
                        "academics" => "/Home/Academics",
                        "faculty" => "/Home/Faculty",
                        "eventsnews" => "/Home/EventsNews",
                        "gallery" => "/Home/Gallery",
                        "facilities" => "/Home/Facilities",
                        "achievements" => "/Home/Achievements",
                        "contact" => "/Home/Contact",
                        "admissions" => "/Admissions",
                        _ => "/Home/" + page.PageName
                    };

                    sb.AppendLine("  <url>");
                    sb.AppendLine($"    <loc>{host}{path}</loc>");
                    sb.AppendLine($"    <lastmod>{DateTime.Now:yyyy-MM-dd}</lastmod>");
                    sb.AppendLine("    <changefreq>weekly</changefreq>");
                    sb.AppendLine("    <priority>0.8</priority>");
                    sb.AppendLine("  </url>");
                }

                sb.AppendLine("</urlset>");
                await System.IO.File.WriteAllTextAsync(sitemapPath, sb.ToString(), Encoding.UTF8);

                TempData["Success"] = "Sitemap.xml rebuilt successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Failed to rebuild sitemap: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
