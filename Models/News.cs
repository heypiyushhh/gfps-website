using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class News
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(250)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(1000)]
        public string Summary { get; set; } = string.Empty; // Short Description

        [Required]
        public string Content { get; set; } = string.Empty; // Full Content

        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty; // Featured Image

        [Required]
        [StringLength(100)]
        public string Author { get; set; } = string.Empty;

        public bool IsFeatured { get; set; } = false;
        public bool IsPublished { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
