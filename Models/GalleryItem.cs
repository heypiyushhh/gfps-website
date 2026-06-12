using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class GalleryItem
    {
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Academics, Sports, Cultural, Campus

        [Required]
        [StringLength(255)]
        public string FilePath { get; set; } = string.Empty;

        public bool IsVideo { get; set; } = false;
        
        [StringLength(255)]
        public string VideoUrl { get; set; } = string.Empty;

        public int? GalleryAlbumId { get; set; }
        public virtual GalleryAlbum? GalleryAlbum { get; set; }

        public bool IsDeleted { get; set; } = false;
    }
}
