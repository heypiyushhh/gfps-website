using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using System.Linq;
using System.Threading.Tasks;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,ContentManager,AdmissionsStaff")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalFaculty = await _context.FacultyMembers.CountAsync();
            ViewBag.TotalPrograms = await _context.AcademicPrograms.CountAsync(p => !p.IsDeleted);
            ViewBag.TotalEvents = await _context.Events.CountAsync(e => !e.IsDeleted);
            ViewBag.TotalNews = await _context.News.CountAsync(n => !n.IsDeleted);
            ViewBag.TotalAlbums = await _context.GalleryAlbums.CountAsync(a => !a.IsDeleted);
            ViewBag.TotalGalleryImages = await _context.GalleryItems.CountAsync(i => !i.IsDeleted && !i.IsVideo);
            ViewBag.TotalFacilities = await _context.Facilities.CountAsync(f => !f.IsDeleted);
            ViewBag.TotalAchievements = await _context.Achievements.CountAsync(a => !a.IsDeleted);

            ViewBag.TotalApplications = await _context.AdmissionApplications.CountAsync();
            ViewBag.TotalInquiries = await _context.ContactInquiries.CountAsync();
            ViewBag.UnreadInquiries = await _context.ContactInquiries.CountAsync(q => !q.IsRead);
            ViewBag.MediaFilesCount = await _context.MediaFiles.CountAsync();

            var recentInquiries = await _context.ContactInquiries
                .OrderByDescending(q => q.SubmittedAt)
                .Take(5)
                .ToListAsync();

            var recentApplications = await _context.AdmissionApplications
                .OrderByDescending(a => a.SubmittedAt)
                .Take(5)
                .ToListAsync();

            ViewBag.RecentInquiries = recentInquiries;
            ViewBag.RecentApplications = recentApplications;

            return View();
        }
    }
}
