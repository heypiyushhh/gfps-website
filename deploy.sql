CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;
CREATE TABLE "AcademicPrograms" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AcademicPrograms" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Department" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "FullDescription" TEXT NOT NULL,
    "ClassOrGrade" TEXT NOT NULL,
    "Subject" TEXT NOT NULL,
    "CurriculumJson" TEXT NOT NULL,
    "Icon" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "FeaturedImage" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "Achievements" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Achievements" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "RecipientName" TEXT NOT NULL,
    "Year" TEXT NOT NULL,
    "AchievementDate" TEXT NOT NULL,
    "AchievementType" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "Icon" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "CertificatePath" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "AdmissionApplications" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AdmissionApplications" PRIMARY KEY AUTOINCREMENT,
    "CandidateName" TEXT NOT NULL,
    "DateOfBirth" TEXT NOT NULL,
    "Gender" TEXT NOT NULL,
    "GradeApplied" TEXT NOT NULL,
    "ParentName" TEXT NOT NULL,
    "ParentEmail" TEXT NOT NULL,
    "ParentPhone" TEXT NOT NULL,
    "Address" TEXT NOT NULL,
    "Status" TEXT NOT NULL,
    "SubmittedAt" TEXT NOT NULL
);

CREATE TABLE "AspNetRoles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetRoles" PRIMARY KEY,
    "Name" TEXT NULL,
    "NormalizedName" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL
);

CREATE TABLE "AspNetUsers" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_AspNetUsers" PRIMARY KEY,
    "FullName" TEXT NOT NULL,
    "ProfilePicturePath" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "LastLoginDate" TEXT NULL,
    "UserName" TEXT NULL,
    "NormalizedUserName" TEXT NULL,
    "Email" TEXT NULL,
    "NormalizedEmail" TEXT NULL,
    "EmailConfirmed" INTEGER NOT NULL,
    "PasswordHash" TEXT NULL,
    "SecurityStamp" TEXT NULL,
    "ConcurrencyStamp" TEXT NULL,
    "PhoneNumber" TEXT NULL,
    "PhoneNumberConfirmed" INTEGER NOT NULL,
    "TwoFactorEnabled" INTEGER NOT NULL,
    "LockoutEnd" TEXT NULL,
    "LockoutEnabled" INTEGER NOT NULL,
    "AccessFailedCount" INTEGER NOT NULL
);

CREATE TABLE "AuditLogs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AuditLogs" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "UserEmail" TEXT NOT NULL,
    "Action" TEXT NOT NULL,
    "TableName" TEXT NOT NULL,
    "RecordId" TEXT NOT NULL,
    "OldValues" TEXT NOT NULL,
    "NewValues" TEXT NOT NULL,
    "Timestamp" TEXT NOT NULL
);

CREATE TABLE "ContactInquiries" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ContactInquiries" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "Subject" TEXT NOT NULL,
    "Message" TEXT NOT NULL,
    "SubmittedAt" TEXT NOT NULL,
    "IsRead" INTEGER NOT NULL,
    "ReplyText" TEXT NOT NULL,
    "RepliedAt" TEXT NULL
);

CREATE TABLE "Events" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Events" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "EventDate" TEXT NOT NULL,
    "EventTime" TEXT NOT NULL,
    "Location" TEXT NOT NULL,
    "ShortDescription" TEXT NOT NULL,
    "FullDescription" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "GalleryImagesJson" TEXT NOT NULL,
    "RegistrationLink" TEXT NOT NULL,
    "IsPublished" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "Facilities" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Facilities" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Details" TEXT NOT NULL,
    "Icon" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "FacultyMembers" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FacultyMembers" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Designation" TEXT NOT NULL,
    "Department" TEXT NOT NULL,
    "Bio" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "DisplayOrder" INTEGER NOT NULL
);

CREATE TABLE "GalleryAlbums" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_GalleryAlbums" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CoverImageUrl" TEXT NOT NULL,
    "EventTag" TEXT NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "LoginHistories" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_LoginHistories" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "UserEmail" TEXT NOT NULL,
    "LoginTime" TEXT NOT NULL,
    "IpAddress" TEXT NOT NULL,
    "BrowserAgent" TEXT NOT NULL,
    "IsSuccessful" INTEGER NOT NULL
);

CREATE TABLE "MediaFiles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_MediaFiles" PRIMARY KEY AUTOINCREMENT,
    "FileName" TEXT NOT NULL,
    "FilePath" TEXT NOT NULL,
    "SizeBytes" INTEGER NOT NULL,
    "ContentType" TEXT NOT NULL,
    "UploadedAt" TEXT NOT NULL,
    "UploadedBy" TEXT NOT NULL
);

CREATE TABLE "News" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_News" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Slug" TEXT NOT NULL,
    "PublishDate" TEXT NOT NULL,
    "Summary" TEXT NOT NULL,
    "Content" TEXT NOT NULL,
    "ImagePath" TEXT NOT NULL,
    "Author" TEXT NOT NULL,
    "IsFeatured" INTEGER NOT NULL,
    "IsPublished" INTEGER NOT NULL,
    "IsActive" INTEGER NOT NULL,
    "IsDeleted" INTEGER NOT NULL,
    "CreatedDate" TEXT NOT NULL,
    "UpdatedDate" TEXT NOT NULL
);

CREATE TABLE "SeoMetadata" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_SeoMetadata" PRIMARY KEY AUTOINCREMENT,
    "PageName" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "MetaDescription" TEXT NOT NULL,
    "OpenGraphTitle" TEXT NOT NULL,
    "OpenGraphDescription" TEXT NOT NULL,
    "OpenGraphImage" TEXT NOT NULL,
    "CanonicalUrl" TEXT NOT NULL
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY AUTOINCREMENT,
    "RoleId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "ClaimType" TEXT NULL,
    "ClaimValue" TEXT NULL,
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" TEXT NOT NULL,
    "ProviderKey" TEXT NOT NULL,
    "ProviderDisplayName" TEXT NULL,
    "UserId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" TEXT NOT NULL,
    "RoleId" TEXT NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" TEXT NOT NULL,
    "LoginProvider" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Value" TEXT NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "GalleryItems" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_GalleryItems" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL,
    "Category" TEXT NOT NULL,
    "FilePath" TEXT NOT NULL,
    "IsVideo" INTEGER NOT NULL,
    "VideoUrl" TEXT NOT NULL,
    "GalleryAlbumId" INTEGER NULL,
    "IsDeleted" INTEGER NOT NULL,
    CONSTRAINT "FK_GalleryItems_GalleryAlbums_GalleryAlbumId" FOREIGN KEY ("GalleryAlbumId") REFERENCES "GalleryAlbums" ("Id")
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

CREATE INDEX "IX_GalleryItems_GalleryAlbumId" ON "GalleryItems" ("GalleryAlbumId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260612052250_InitialCreate', '10.0.9');

COMMIT;

