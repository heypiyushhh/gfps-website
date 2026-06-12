using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace gfps.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string EventTime { get; set; } = string.Empty; // e.g. "10:00 AM"

        [Required]
        [StringLength(200)]
        public string Location { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ShortDescription { get; set; } = string.Empty;

        [Required]
        public string FullDescription { get; set; } = string.Empty;

        [StringLength(255)]
        public string ImagePath { get; set; } = string.Empty; // Event Banner Image

        public string GalleryImagesJson { get; set; } = "[]"; // Additional event images

        [StringLength(255)]
        public string RegistrationLink { get; set; } = string.Empty;

        public bool IsPublished { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string Description
        {
            get => ShortDescription;
            set => ShortDescription = value;
        }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
