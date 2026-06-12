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
    public class ContactInquiryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactInquiryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchString, string statusFilter)
        {
            var query = _context.ContactInquiries.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(i => i.Name.Contains(searchString) || i.Email.Contains(searchString) || i.Subject.Contains(searchString) || i.Message.Contains(searchString));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                bool onlyRead = statusFilter == "Read";
                query = query.Where(i => i.IsRead == onlyRead);
            }

            var inquiries = await query.OrderByDescending(i => i.SubmittedAt).ToListAsync();

            ViewBag.SearchString = searchString;
            ViewBag.StatusFilter = statusFilter;

            return View(inquiries);
        }

        public async Task<IActionResult> Details(int id)
        {
            var inquiry = await _context.ContactInquiries.FindAsync(id);
            if (inquiry == null) return NotFound();

            if (!inquiry.IsRead)
            {
                inquiry.IsRead = true;
                await _context.SaveChangesAsync();
            }

            return View(inquiry);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(int id, string replyText)
        {
            var inquiry = await _context.ContactInquiries.FindAsync(id);
            if (inquiry != null && !string.IsNullOrEmpty(replyText))
            {
                inquiry.ReplyText = replyText;
                inquiry.RepliedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Reply tracked successfully.";
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var inquiries = await _context.ContactInquiries.OrderByDescending(i => i.SubmittedAt).ToListAsync();

            var csv = new StringBuilder();
            csv.AppendLine("Inquiry ID,Sender Name,Sender Email,Subject,Message,Submitted At,Read Status,Reply Text,Replied At");

            foreach (var inq in inquiries)
            {
                string cleanMsg = inq.Message.Replace("\"", "\"\"");
                string cleanReply = inq.ReplyText.Replace("\"", "\"\"");
                csv.AppendLine($"{inq.Id},\"{inq.Name}\",\"{inq.Email}\",\"{inq.Subject}\",\"{cleanMsg}\",{inq.SubmittedAt:yyyy-MM-dd HH:mm:ss},{inq.IsRead},\"{cleanReply}\",{(inq.RepliedAt.HasValue ? inq.RepliedAt.Value.ToString("yyyy-MM-dd HH:mm:ss") : "")}");
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());
            return File(buffer, "text/csv", $"Contact_Inquiries_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }
    }
}
