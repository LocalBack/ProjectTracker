using MediatR;
using ProjectTracker.Core.Events; // Event'in bulunduðu assembly  
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Data.Seed;
using ProjectTracker.Service.Events.Handlers;
using ProjectTracker.Service.Implementations;
using ProjectTracker.Service.Mapping;
using ProjectTracker.Service.Services.Implementations;
using ProjectTracker.Service.Services.Interfaces;
using ProjectTracker.Web.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Note: No need to add AppDbContext separately as AddDbContext already registers it

// Identity Configuration
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User settings
    options.User.RequireUniqueEmail = true;

    // Lockout settings
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Add after Identity configuration
builder.Services.AddAuthorization(options =>
{
    // Existing fallback policy
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    // Custom policies
    options.AddPolicy("EmployeeOnly", policy =>
        policy.RequireRole("Employee", "Manager", "Admin"));

    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireRole("Manager", "Admin"));

    options.AddPolicy("CanManageProjects", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Manager") ||
            context.User.IsInRole("Admin")));

    options.AddPolicy("CanViewReports", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("User") ||
            context.User.IsInRole("Employee") ||
            context.User.IsInRole("Manager") ||
            context.User.IsInRole("Admin")));
});

// Cookie Configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

// Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IWorkLogService, WorkLogService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IMaintenanceScheduleService, MaintenanceScheduleService>();

// Authorization Configuration
builder.Services.AddAuthorization(options =>
{
    // This policy makes the ENTIRE APPLICATION require login
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddScoped<IAuthorizationHandler, WorkLogAuthorizationHandler>();

// Add before builder.Build()
builder.Services.AddScoped<IUserDashboardService, UserDashboardService>(); // if you have this service

// In your Program.cs, modify your DbContext configuration:
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer("DefaultConnection")
           .LogTo(Console.WriteLine, LogLevel.Information)
           .EnableSensitiveDataLogging();
});

builder.Services.AddMediatR(typeof(EmployeeUpdatedEventHandler).Assembly);
builder.Services.AddScoped<IUserProjectService, UserProjectService>();



builder.Services.AddScoped<IUserProjectSyncService, UserProjectSyncService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();










var app = builder.Build();

// Seed Data - UPDATED VERSION
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // Call SeedAsync with IServiceProvider
        await IdentitySeed.SeedAsync(services);

        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Identity seed data successfully created!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding identity data!");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication middleware - Must be before Authorization!
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();