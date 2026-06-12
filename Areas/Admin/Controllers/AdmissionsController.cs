using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace gfps.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "SuperAdmin,Admin,AdmissionsStaff")]
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string statusFilter, string gradeFilter)
        {
            var query = _context.AdmissionApplications.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(a => a.CandidateName.Contains(searchString) || a.ParentName.Contains(searchString) || a.ParentEmail.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(a => a.Status == statusFilter);
            }

            if (!string.IsNullOrEmpty(gradeFilter))
            {
                query = query.Where(a => a.GradeApplied == gradeFilter);
            }

            var applications = await query.OrderByDescending(a => a.SubmittedAt).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.GradeFilter = gradeFilter;

            return View(applications);
        }

        public async Task<IActionResult> Details(int id)
        {
            var application = await _context.AdmissionApplications.FindAsync(id);
            if (application == null) return NotFound();
            return View(application);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var application = await _context.AdmissionApplications.FindAsync(id);
            if (application != null && !string.IsNullOrEmpty(status))
            {
                application.Status = status;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Application status updated to '{status}'.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var applications = await _context.AdmissionApplications.OrderByDescending(a => a.SubmittedAt).ToListAsync();
            
            var csv = new StringBuilder();
            csv.AppendLine("Application ID,Candidate Name,Date of Birth,Gender,Grade Applied,Parent Name,Parent Email,Parent Phone,Address,Status,Submitted At");

            foreach (var app in applications)
            {
                csv.AppendLine($"{app.Id},\"{app.CandidateName}\",{app.DateOfBirth:yyyy-MM-dd},\"{app.Gender}\",\"{app.GradeApplied}\",\"{app.ParentName}\",\"{app.ParentEmail}\",\"{app.ParentPhone}\",\"{app.Address.Replace("\"", "\"\"")}\",\"{app.Status}\",{app.SubmittedAt:yyyy-MM-dd HH:mm:ss}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", $"Admissions_Applications_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
    }
}
