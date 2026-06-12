using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class Achievement
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string RecipientName { get; set; } = string.Empty; // Student/Teacher Name

        [Required]
        [StringLength(10)]
        public string Year { get; set; } = string.Empty;

        [Required]
        public DateTime AchievementDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string AchievementType { get; set; } = string.Empty; // Student/Teacher/School

        [Required]
        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // e.g. Student, Accolades, Athletics

        [StringLength(50)]
        public string Icon { get; set; } = "emoji_events"; // Google material icon name

        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty;

        [StringLength(255)]
        public string CertificatePath { get; set; } = string.Empty; // Optional certificate path

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
