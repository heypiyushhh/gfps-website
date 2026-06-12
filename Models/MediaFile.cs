using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class MediaFile
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        public long SizeBytes { get; set; }

        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string UploadedBy { get; set; } = string.Empty;
    }
}
