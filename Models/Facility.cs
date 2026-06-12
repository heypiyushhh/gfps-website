using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class Facility
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty; // Short Description

        [Required]
        [StringLength(1500)]
        public string Details { get; set; } = string.Empty; // Full Description/Details

        [StringLength(50)]
        public string Icon { get; set; } = "domain"; // Google material icon name

        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
