using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class FacultyMember
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Designation { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty; // e.g. Science, Mathematics, Humanities, Arts

        [StringLength(1000)]
        public string Bio { get; set; } = string.Empty;

        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty;

        public int DisplayOrder { get; set; } = 0;
    }
}
