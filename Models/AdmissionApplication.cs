using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class AdmissionApplication
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Candidate Full Name")]
        public string CandidateName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Grade Applied For")]
        public string GradeApplied { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Display(Name = "Parent/Guardian Name")]
        public string ParentName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Parent Email")]
        public string ParentEmail { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(20)]
        [Display(Name = "Parent Phone")]
        public string ParentPhone { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Submitted"; // Submitted, Reviewing, Accepted, Rejected

        public DateTime SubmittedAt { get; set; } = DateTime.Now;
    }
}
