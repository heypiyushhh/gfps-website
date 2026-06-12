using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string UserEmail { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty; // Create, Update, Delete

        [Required]
        [StringLength(100)]
        public string TableName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string RecordId { get; set; } = string.Empty;

        public string OldValues { get; set; } = string.Empty; // JSON format

        public string NewValues { get; set; } = string.Empty; // JSON format

        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}
