using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace gfps.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [StringLength(255)]
        public string ProfilePicturePath { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }
    }
}
