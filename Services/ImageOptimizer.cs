using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;

namespace gfps.Services
{
    public class ImageOptimizer
    {
        public bool IsValidImage(IFormFile file)
        {
            if (file == null) return false;

            // Check file length
            if (file.Length > 10 * 1024 * 1024) return false; // 10MB Limit

            // Verify extension
            string ext = Path.GetExtension(file.FileName).ToLower();
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".webp")
            {
                return false;
            }

            // Verify content type
            string contentType = file.ContentType.ToLower();
            if (!contentType.StartsWith("image/"))
            {
                return false;
            }

            return true;
        }

        public async Task<string> SaveAndOptimizeImageAsync(IFormFile file, string targetFolder)
        {
            if (!IsValidImage(file))
            {
                throw new InvalidOperationException("Invalid image file format or size.");
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(targetFolder, fileName);

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // In production, we can use SixLabors.ImageSharp to resize and compress
            // For out-of-the-box local running, we save directly to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + fileName;
        }
    }
}
