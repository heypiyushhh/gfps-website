using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class LoginHistory
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        public DateTime LoginTime { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string IpAddress { get; set; } = string.Empty;

        [StringLength(500)]
        public string BrowserAgent { get; set; } = string.Empty;

        public bool IsSuccessful { get; set; }
    }
}
