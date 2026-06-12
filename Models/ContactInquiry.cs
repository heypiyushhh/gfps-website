using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class ContactInquiry
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string Subject { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Message { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        public string ReplyText { get; set; } = string.Empty;

        public DateTime? RepliedAt { get; set; }
    }
}
