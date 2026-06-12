using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class SeoMetadata
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string PageName { get; set; } = string.Empty; // e.g. Home, About, Academics, Faculty, etc.

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string MetaDescription { get; set; } = string.Empty;

        [StringLength(150)]
        public string OpenGraphTitle { get; set; } = string.Empty;

        [StringLength(255)]
        public string OpenGraphDescription { get; set; } = string.Empty;

        [StringLength(255)]
        public string OpenGraphImage { get; set; } = string.Empty;

        [StringLength(255)]
        public string CanonicalUrl { get; set; } = string.Empty;
    }
}
