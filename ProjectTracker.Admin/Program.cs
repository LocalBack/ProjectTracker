using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;              // BackgroundService
using Microsoft.Extensions.Logging;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Data.Seed;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.Implementations;
using ProjectTracker.Service.Mapping;
using ProjectTracker.Service.Services.Implementations;
using ProjectTracker.Service.Services.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

/*──────────────────────────── 1) DbContext ───────────────────────────*/
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Missing conn-string");

builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlServer(cs, sql =>
    {
        sql.CommandTimeout(120);                // CHANGED: 30 s → 120 s
        sql.EnableRetryOnFailure(5);            // NEW: transient retry
    });
});

/* Localization */
builder.Services.AddLocalization(opt => opt.ResourcesPath = "Resources");
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supported = new[] { "tr-TR", "en-US" }
        .Select(c => new CultureInfo(c)).ToList();
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supported;
    options.SupportedUICultures = supported;
});

/*──────────────────────────── 2) Identity  ───────────────────────────*/
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

/*──────────────────────────── 3) Authorization ───────────────────────*/
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build());

/*──────────────────────────── 4) Razor Pages ─────────────────────────*/
builder.Services.AddRazorPages(opt =>
{
    opt.Conventions.AuthorizeFolder("/", "AdminOnly");
    opt.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

/*──────────────────────────── 5) Mapping / Services ──────────────────*/
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IMaintenanceScheduleService, MaintenanceScheduleService>();
builder.Services.AddHostedService<MaintenanceNotificationService>();
builder.Services.AddScoped<IProjectDashboardService, ProjectDashboardService>();

/*──────────────────────────── 6) HttpClient  (NEW) ───────────────────
   Yavaş/arıza yapan dış API çağrıları “task cancelled” üretmesin */
builder.Services.AddHttpClient("Default", c =>
{
    c.Timeout = TimeSpan.FromSeconds(100);      // default 100 s > çoğu senaryo
}).SetHandlerLifetime(TimeSpan.FromMinutes(5)); // soket sızıntısına karşı

/*──────────────────────────── 7) Host seçenekleri (NEW) ──────────────
   Graceful shutdown → uzun süren BG görevleri iptal edilmeden tamamlayabilsin */
builder.Services.Configure<HostOptions>(o =>
{
    o.ShutdownTimeout = TimeSpan.FromSeconds(30);
});

/*──────────────────────────── 8) Identity ayrıntılı ayarlar ──────────*/
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

var app = builder.Build();

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

/*────────────────────── 9) Global exception / cancellation log (NEW) ─*/
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async ctx =>
    {
        var ex = ctx.Features.Get<IExceptionHandlerPathFeature>()?.Error;
        if (ex is TaskCanceledException)
            app.Logger.LogWarning(ex, "Request or background task was cancelled.");
        else
            app.Logger.LogError(ex, "Unhandled exception");

        await Results.Problem().ExecuteAsync(ctx);
    });
});

/*────────────────────────── 10) HTTP pipeline ───────────────────────*/
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseHsts();
}

await IdentitySeed.SeedAsync(app.Services);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();
