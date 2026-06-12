using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace gfps.Services
{
    public static class SafeFileUpload
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private static readonly string[] AllowedImageMimeTypes = { "image/jpeg", "image/png", "image/gif", "image/webp", "image/pjpeg", "image/x-png" };

        private static readonly string[] AllowedDocExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private static readonly string[] AllowedDocMimeTypes = { "application/pdf", "image/jpeg", "image/png", "image/pjpeg", "image/x-png" };

        private const long MaxImageSize = 5 * 1024 * 1024; // 5 MB
        private const long MaxDocSize = 10 * 1024 * 1024; // 10 MB

        /// <summary>
        /// Validates and saves an uploaded image.
        /// </summary>
        public static async Task<string> SaveImageAsync(IFormFile file, string targetFolder)
        {
            ValidateFile(file, AllowedImageExtensions, AllowedImageMimeTypes, MaxImageSize, "Image");

            return await SaveToDiskAsync(file, targetFolder);
        }

        /// <summary>
        /// Validates and saves an uploaded certificate document (PDF or image).
        /// </summary>
        public static async Task<string> SaveCertificateAsync(IFormFile file, string targetFolder)
        {
            ValidateFile(file, AllowedDocExtensions, AllowedDocMimeTypes, MaxDocSize, "Certificate Document");

            return await SaveToDiskAsync(file, targetFolder);
        }

        private static void ValidateFile(IFormFile file, string[] allowedExtensions, string[] allowedMimeTypes, long maxSizeBytes, string fileTypeFriendlyName)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException($"No {fileTypeFriendlyName.ToLower()} file was provided or the file is empty.");
            }

            // 1. Validate File Size
            if (file.Length > maxSizeBytes)
            {
                double maxSizeMb = maxSizeBytes / (1024.0 * 1024.0);
                throw new InvalidOperationException($"The uploaded {fileTypeFriendlyName.ToLower()} exceeds the maximum size limit of {maxSizeMb:F0}MB.");
            }

            // 2. Validate Extension
            string ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext) || !allowedExtensions.Contains(ext.ToLowerInvariant()))
            {
                throw new InvalidOperationException($"The uploaded file extension '{ext}' is not permitted for a {fileTypeFriendlyName.ToLower()}. Permitted extensions: {string.Join(", ", allowedExtensions)}");
            }

            // 3. Validate Mime Type
            string contentType = file.ContentType;
            if (string.IsNullOrEmpty(contentType) || !allowedMimeTypes.Contains(contentType.ToLowerInvariant()))
            {
                throw new InvalidOperationException($"The uploaded file content type '{contentType}' is not permitted for a {fileTypeFriendlyName.ToLower()}.");
            }

            // 4. Double check by looking at the start of content-type
            if (fileTypeFriendlyName == "Image" && !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Invalid image mime type detected.");
            }
        }

        private static async Task<string> SaveToDiskAsync(IFormFile file, string targetFolder)
        {
            string cleanExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string uniqueFileName = Guid.NewGuid().ToString("N") + cleanExtension;

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            string fullPath = Path.Combine(targetFolder, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "/images/" + uniqueFileName;
        }
    }
}
