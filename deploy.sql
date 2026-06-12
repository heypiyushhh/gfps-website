IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [AcademicPrograms] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Department] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [FullDescription] nvarchar(max) NOT NULL,
    [ClassOrGrade] nvarchar(50) NOT NULL,
    [Subject] nvarchar(100) NOT NULL,
    [CurriculumJson] nvarchar(max) NOT NULL,
    [Icon] nvarchar(50) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [FeaturedImage] nvarchar(255) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_AcademicPrograms] PRIMARY KEY ([Id])
);

CREATE TABLE [Achievements] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(150) NOT NULL,
    [RecipientName] nvarchar(150) NOT NULL,
    [Year] nvarchar(10) NOT NULL,
    [AchievementDate] datetime2 NOT NULL,
    [AchievementType] nvarchar(50) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [Icon] nvarchar(50) NOT NULL,
    [ImagePath] nvarchar(255) NOT NULL,
    [CertificatePath] nvarchar(255) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Achievements] PRIMARY KEY ([Id])
);

CREATE TABLE [AdmissionApplications] (
    [Id] int NOT NULL IDENTITY,
    [CandidateName] nvarchar(100) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [GradeApplied] nvarchar(50) NOT NULL,
    [ParentName] nvarchar(100) NOT NULL,
    [ParentEmail] nvarchar(100) NOT NULL,
    [ParentPhone] nvarchar(20) NOT NULL,
    [Address] nvarchar(500) NOT NULL,
    [Status] nvarchar(50) NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AdmissionApplications] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [FullName] nvarchar(100) NOT NULL,
    [ProfilePicturePath] nvarchar(255) NOT NULL,
    [IsActive] bit NOT NULL,
    [LastLoginDate] datetime2 NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(100) NOT NULL,
    [UserEmail] nvarchar(100) NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [TableName] nvarchar(100) NOT NULL,
    [RecordId] nvarchar(100) NOT NULL,
    [OldValues] nvarchar(max) NOT NULL,
    [NewValues] nvarchar(max) NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id])
);

CREATE TABLE [ContactInquiries] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Email] nvarchar(100) NOT NULL,
    [Subject] nvarchar(150) NOT NULL,
    [Message] nvarchar(2000) NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [IsRead] bit NOT NULL,
    [ReplyText] nvarchar(max) NOT NULL,
    [RepliedAt] datetime2 NULL,
    CONSTRAINT [PK_ContactInquiries] PRIMARY KEY ([Id])
);

CREATE TABLE [Events] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [EventDate] datetime2 NOT NULL,
    [EventTime] nvarchar(50) NOT NULL,
    [Location] nvarchar(200) NOT NULL,
    [ShortDescription] nvarchar(500) NOT NULL,
    [FullDescription] nvarchar(max) NOT NULL,
    [ImagePath] nvarchar(255) NOT NULL,
    [GalleryImagesJson] nvarchar(max) NOT NULL,
    [RegistrationLink] nvarchar(255) NOT NULL,
    [IsPublished] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([Id])
);

CREATE TABLE [Facilities] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [Details] nvarchar(1500) NOT NULL,
    [Icon] nvarchar(50) NOT NULL,
    [ImagePath] nvarchar(255) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Facilities] PRIMARY KEY ([Id])
);

CREATE TABLE [FacultyMembers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Designation] nvarchar(100) NOT NULL,
    [Department] nvarchar(50) NOT NULL,
    [Bio] nvarchar(1000) NOT NULL,
    [ImagePath] nvarchar(255) NOT NULL,
    [DisplayOrder] int NOT NULL,
    CONSTRAINT [PK_FacultyMembers] PRIMARY KEY ([Id])
);

CREATE TABLE [GalleryAlbums] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(150) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    [CoverImageUrl] nvarchar(255) NOT NULL,
    [EventTag] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_GalleryAlbums] PRIMARY KEY ([Id])
);

CREATE TABLE [LoginHistories] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(100) NOT NULL,
    [UserEmail] nvarchar(100) NOT NULL,
    [LoginTime] datetime2 NOT NULL,
    [IpAddress] nvarchar(50) NOT NULL,
    [BrowserAgent] nvarchar(500) NOT NULL,
    [IsSuccessful] bit NOT NULL,
    CONSTRAINT [PK_LoginHistories] PRIMARY KEY ([Id])
);

CREATE TABLE [MediaFiles] (
    [Id] int NOT NULL IDENTITY,
    [FileName] nvarchar(255) NOT NULL,
    [FilePath] nvarchar(255) NOT NULL,
    [SizeBytes] bigint NOT NULL,
    [ContentType] nvarchar(100) NOT NULL,
    [UploadedAt] datetime2 NOT NULL,
    [UploadedBy] nvarchar(100) NOT NULL,
    CONSTRAINT [PK_MediaFiles] PRIMARY KEY ([Id])
);

CREATE TABLE [News] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(200) NOT NULL,
    [Slug] nvarchar(250) NOT NULL,
    [PublishDate] datetime2 NOT NULL,
    [Summary] nvarchar(1000) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [ImagePath] nvarchar(255) NOT NULL,
    [Author] nvarchar(100) NOT NULL,
    [IsFeatured] bit NOT NULL,
    [IsPublished] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [IsDeleted] bit NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    CONSTRAINT [PK_News] PRIMARY KEY ([Id])
);

CREATE TABLE [SeoMetadata] (
    [Id] int NOT NULL IDENTITY,
    [PageName] nvarchar(100) NOT NULL,
    [Title] nvarchar(150) NOT NULL,
    [MetaDescription] nvarchar(255) NOT NULL,
    [OpenGraphTitle] nvarchar(150) NOT NULL,
    [OpenGraphDescription] nvarchar(255) NOT NULL,
    [OpenGraphImage] nvarchar(255) NOT NULL,
    [CanonicalUrl] nvarchar(255) NOT NULL,
    CONSTRAINT [PK_SeoMetadata] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [GalleryItems] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(150) NOT NULL,
    [Category] nvarchar(50) NOT NULL,
    [FilePath] nvarchar(255) NOT NULL,
    [IsVideo] bit NOT NULL,
    [VideoUrl] nvarchar(255) NOT NULL,
    [GalleryAlbumId] int NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GalleryItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GalleryItems_GalleryAlbums_GalleryAlbumId] FOREIGN KEY ([GalleryAlbumId]) REFERENCES [GalleryAlbums] ([Id])
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

CREATE INDEX [IX_GalleryItems_GalleryAlbumId] ON [GalleryItems] ([GalleryAlbumId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260612063202_InitialSqlServerCreate', N'10.0.9');

COMMIT;
GO

