using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace gfps.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private async Task SetSeoHeadersAsync(string pageName)
        {
            var seo = await _context.SeoMetadata.FirstOrDefaultAsync(m => m.PageName == pageName);
            if (seo != null)
            {
                ViewBag.MetaTitle = seo.Title;
                ViewBag.MetaDescription = seo.MetaDescription;
                ViewBag.OgTitle = seo.OpenGraphTitle;
                ViewBag.OgDescription = seo.OpenGraphDescription;
                ViewBag.OgImage = seo.OpenGraphImage;
                ViewBag.CanonicalUrl = seo.CanonicalUrl;
            }
            else
            {
                ViewBag.MetaTitle = "Green Field Public School | Empowering Future Leaders";
                ViewBag.MetaDescription = "A premium educational ecosystem designed to foster innovation, critical thinking, and holistic development.";
            }
        }

        public async Task<IActionResult> Index()
        {
            await SetSeoHeadersAsync("Home");
            
            // Get data for homepage
            ViewBag.AchievementsCount = await _context.Achievements.CountAsync(a => !a.IsDeleted && a.IsActive);
            ViewBag.FacultyCount = await _context.FacultyMembers.CountAsync();

            return View();
        }

        public async Task<IActionResult> About()
        {
            await SetSeoHeadersAsync("About");
            return View();
        }

        public async Task<IActionResult> Academics()
        {
            await SetSeoHeadersAsync("Academics");
            var programs = await _context.AcademicPrograms.Where(p => !p.IsDeleted && p.IsActive).OrderBy(p => p.DisplayOrder).ToListAsync();
            return View(programs);
        }

        public async Task<IActionResult> Faculty(string department)
        {
            await SetSeoHeadersAsync("Faculty");

            var query = _context.FacultyMembers.AsQueryable();
            if (!string.IsNullOrEmpty(department) && department != "All")
            {
                query = query.Where(f => f.Department == department);
            }

            var list = await query.OrderBy(f => f.DisplayOrder).ToListAsync();
            ViewBag.SelectedDepartment = department ?? "All";
            
            return View(list);
        }

        public async Task<IActionResult> EventsNews()
        {
            await SetSeoHeadersAsync("EventsNews");
            
            ViewBag.Events = await _context.Events.Where(e => e.IsPublished && !e.IsDeleted && e.IsActive).OrderByDescending(e => e.EventDate).ToListAsync();
            var news = await _context.News.Where(n => n.IsPublished && !n.IsDeleted && n.IsActive).OrderByDescending(n => n.PublishDate).ToListAsync();
            
            return View(news);
        }

        public async Task<IActionResult> Gallery(string category)
        {
            await SetSeoHeadersAsync("Gallery");

            var query = _context.GalleryItems.Where(i => !i.IsDeleted && (i.GalleryAlbum == null || (i.GalleryAlbum.IsActive && !i.GalleryAlbum.IsDeleted))).AsQueryable();
            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(i => i.Category == category);
            }

            var list = await query.ToListAsync();
            ViewBag.SelectedCategory = category ?? "All";

            return View(list);
        }

        public async Task<IActionResult> Facilities()
        {
            await SetSeoHeadersAsync("Facilities");
            var list = await _context.Facilities.Where(f => !f.IsDeleted && f.IsActive).OrderBy(f => f.DisplayOrder).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Achievements()
        {
            await SetSeoHeadersAsync("Achievements");
            var list = await _context.Achievements.Where(a => !a.IsDeleted && a.IsActive).OrderByDescending(a => a.Year).ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> Contact()
        {
            await SetSeoHeadersAsync("Contact");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitContact(ContactInquiry inquiry)
        {
            if (ModelState.IsValid)
            {
                inquiry.SubmittedAt = DateTime.Now;
                inquiry.IsRead = false;
                _context.ContactInquiries.Add(inquiry);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Your message was sent successfully. We will get in touch soon!";
                return RedirectToAction(nameof(Contact));
            }
            TempData["Error"] = "Failed to send message. Please check the form errors.";
            return View("Contact", inquiry);
        }

        [Route("Home/Error/{statusCode?}")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            var model = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                StatusCode = statusCode
            };

            var exceptionHandlerPathFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature?.Error != null)
            {
                _logger.LogError(exceptionHandlerPathFeature.Error, $"Unhandled exception occurred on path: {exceptionHandlerPathFeature.Path}");
            }
            else if (statusCode.HasValue)
            {
                _logger.LogWarning($"HTTP Error {statusCode} occurred for request ID: {model.RequestId}");
            }

            return View(model);
        }
    }
}
