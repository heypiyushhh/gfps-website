using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class GalleryAlbum
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(255)]
        public string CoverImageUrl { get; set; } = string.Empty;

        [StringLength(100)]
        public string EventTag { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;

        public virtual ICollection<GalleryItem> GalleryItems { get; set; } = new List<GalleryItem>();
    }
}
