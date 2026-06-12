using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using gfps.Data;
using gfps.Models;
using gfps.Services;
using System;

var builder = WebApplication.CreateBuilder(args);

// Configure Logging
builder.Logging.AddConsole();
builder.Logging.AddDebug();
var logPath = System.IO.Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "App_Data", "logs", "error-log.txt");
builder.Logging.AddFile(logPath);

// 1. Add DB Context with SQL Server exclusively
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' was not found in configuration.");
}
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Add ASP.NET Core Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Simple passwords for local test, customizable for production
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 3. Configure Login Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
    options.ExpireTimeSpan = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = ".SmartStudies.AuthCookie";
});

// Configure Antiforgery cookie security settings
builder.Services.AddAntiforgery(options =>
{
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = ".SmartStudies.Antiforgery";
});

// 4. Register system services
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// 5. Database Initialization, Connectivity Verification & Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    logger.LogInformation("Verifying database connectivity to SQL Server...");
    try
    {
        logger.LogInformation("Applying Entity Framework migrations to SQL Server...");
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migration and schema checks succeeded.");

        logger.LogInformation("Seeding database values...");
        await DbInitializer.InitializeAsync(services);
        logger.LogInformation("Database seeding completed successfully.");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "CRITICAL: Database connection, migration, or seeding failed! Detailed diagnostics logged.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 6. Map Administrative Area Route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

// 7. Map Default Public Route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
