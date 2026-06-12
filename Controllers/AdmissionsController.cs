using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using System;
using System.Threading.Tasks;

namespace gfps.Controllers
{
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class AdmissionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdmissionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private async Task SetSeoHeadersAsync()
        {
            var seo = await _context.SeoMetadata.FirstOrDefaultAsync(m => m.PageName == "Admissions");
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
                ViewBag.MetaTitle = "Admissions | Green Field Public School";
                ViewBag.MetaDescription = "Secure your child's future. Read about our admissions journey, fees, and requirements.";
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            await SetSeoHeadersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(AdmissionApplication application)
        {
            if (ModelState.IsValid)
            {
                application.Status = "Submitted";
                application.SubmittedAt = DateTime.Now;
                _context.AdmissionApplications.Add(application);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Your admission application has been submitted successfully! Our admissions staff will review it and contact you via email.";
                return RedirectToAction(nameof(Index));
            }

            await SetSeoHeadersAsync();
            TempData["Error"] = "Failed to submit application. Please review the highlighted errors.";
            return View("Index", application);
        }
    }
}
