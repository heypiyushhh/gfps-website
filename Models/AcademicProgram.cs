using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class AcademicProgram
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty; // e.g. Science, Commerce, Humanities

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty; // Reused as Short Description

        [Required]
        public string FullDescription { get; set; } = string.Empty; // Rich Text

        [Required]
        [StringLength(50)]
        public string ClassOrGrade { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Subject { get; set; } = string.Empty;

        public string CurriculumJson { get; set; } = "[]"; // List of curriculum topics

        [StringLength(50)]
        public string Icon { get; set; } = "school"; // Google material icon name

        public int DisplayOrder { get; set; } = 0;

        [StringLength(255)]
        public string FeaturedImage { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
