using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using gfps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace gfps.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor? _httpContextAccessor;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor? httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public DbSet<FacultyMember> FacultyMembers { get; set; }
        public DbSet<AcademicProgram> AcademicPrograms { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<GalleryItem> GalleryItems { get; set; }
        public DbSet<GalleryAlbum> GalleryAlbums { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ContactInquiry> ContactInquiries { get; set; }
        public DbSet<AdmissionApplication> AdmissionApplications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<LoginHistory> LoginHistories { get; set; }
        public DbSet<SeoMetadata> SeoMetadata { get; set; }
        public DbSet<MediaFile> MediaFiles { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var auditEntries = OnBeforeSaveChanges();
            var result = await base.SaveChangesAsync(cancellationToken);
            await OnAfterSaveChanges(auditEntries);
            return result;
        }

        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();

            // Get current user information
            string userId = "System";
            string userEmail = "system@gfps.edu";

            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
            {
                var claimsPrincipal = _httpContextAccessor.HttpContext.User;
                userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                userEmail = claimsPrincipal.FindFirst(ClaimTypes.Name)?.Value ?? claimsPrincipal.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
            }

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is AuditLog || entry.Entity is LoginHistory || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;

                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Entity.GetType().Name,
                    UserId = userId,
                    UserEmail = userEmail
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue ?? "";
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.AuditType = "Create";
                            auditEntry.NewValues[propertyName] = property.CurrentValue ?? "";
                            break;

                        case EntityState.Deleted:
                            auditEntry.AuditType = "Delete";
                            auditEntry.OldValues[propertyName] = property.OriginalValue ?? "";
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.AuditType = "Update";
                                auditEntry.OldValues[propertyName] = property.OriginalValue ?? "";
                                auditEntry.NewValues[propertyName] = property.CurrentValue ?? "";
                            }
                            break;
                    }
                }
            }

            return auditEntries;
        }

        private async Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return;

            foreach (var auditEntry in auditEntries)
            {
                // For new entities, we get the generated primary key after SaveChanges
                foreach (var prop in auditEntry.Entry.Properties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue ?? "";
                    }
                }

                var log = new AuditLog
                {
                    UserId = auditEntry.UserId,
                    UserEmail = auditEntry.UserEmail,
                    Action = auditEntry.AuditType,
                    TableName = auditEntry.TableName,
                    RecordId = JsonSerializer.Serialize(auditEntry.KeyValues),
                    OldValues = auditEntry.OldValues.Count == 0 ? string.Empty : JsonSerializer.Serialize(auditEntry.OldValues),
                    NewValues = auditEntry.NewValues.Count == 0 ? string.Empty : JsonSerializer.Serialize(auditEntry.NewValues),
                    Timestamp = DateTime.Now
                };

                AuditLogs.Add(log);
            }

            await base.SaveChangesAsync();
        }
    }

    internal class AuditEntry
    {
        public Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry Entry { get; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string AuditType { get; set; } = string.Empty;
        public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
        public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

        public AuditEntry(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
        {
            Entry = entry;
        }
    }
}
