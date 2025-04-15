using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PersonalChallengePlatform.Data;
using PersonalChallengePlatform.Models;
using PersonalChallengePlatform.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseKestrel(options =>
{
    options.ListenAnyIP(5002); // HTTP only for development
    options.Limits.MaxRequestBodySize = 52428800; // 50MB
    options.Limits.MaxConcurrentConnections = 100;
    options.Limits.MaxConcurrentUpgradedConnections = 100;
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(1);
});

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookie policy
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.Cookie.HttpOnly = true;
});

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add DatabaseSeeder
builder.Services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

// Add NotFoundErrorService
builder.Services.AddScoped<INotFoundErrorService, NotFoundErrorService>();

// Add AchievementService
builder.Services.AddScoped<IAchievementService, AchievementService>();

// Configure error notifications
var errorConfig = new ErrorNotificationLoggerConfiguration
{
    Environment = builder.Environment.EnvironmentName,
    NotificationType = builder.Configuration["ErrorNotifications:Type"] ?? "Slack",
    WebhookUrl = builder.Configuration["ErrorNotifications:SlackWebhookUrl"] ?? string.Empty,
    EmailEndpoint = builder.Configuration["ErrorNotifications:EmailEndpoint"] ?? string.Empty,
    EmailRecipients = builder.Configuration.GetSection("ErrorNotifications:EmailRecipients").Get<string[]>() ?? Array.Empty<string>()
};

// Only add error notifications in production
if (builder.Environment.IsProduction())
{
    builder.Logging.AddProvider(new ErrorNotificationLoggerProvider(errorConfig));
}

var app = builder.Build();

// Ensure database is created and migrated
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        var seeder = services.GetRequiredService<IDatabaseSeeder>();
        
        context.Database.Migrate(); // Apply any pending migrations

        // Seed Categories
        if (!context.Categories.Any())
        {
            context.Categories.AddRange(
                new Category { Name = "Fitness", Description = "Physical fitness and exercise goals" },
                new Category { Name = "Learning", Description = "Educational and skill development goals" },
                new Category { Name = "Career", Description = "Professional development and career goals" },
                new Category { Name = "Personal Development", Description = "Self-improvement and personal growth" },
                new Category { Name = "Health", Description = "Health and wellness goals" }
            );
            context.SaveChanges();
        }

        // Seed sample users
        await seeder.SeedSampleUsersAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Add error handling middleware
app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode >= 400)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        var statusCode = context.Response.StatusCode;
        var path = context.Request.Path;
        var method = context.Request.Method;
        var timestamp = DateTime.UtcNow;

        logger.LogError(
            "HTTP {StatusCode} error occurred - Method: {Method}, Path: {Path}, Timestamp: {Timestamp}",
            statusCode,
            method,
            path,
            timestamp
        );

        switch (statusCode)
        {
            case 403:
                context.Request.Path = "/Home/Forbidden";
                await next();
                break;
            case 404:
                context.Request.Path = "/Home/NotFound";
                await next();
                break;
            case 500:
                context.Request.Path = "/Home/InternalServerError";
                await next();
                break;
        }
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add fallback route
app.MapFallbackToController("NotFound", "Home");

app.Run();