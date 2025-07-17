using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
var cs = builder.Configuration.GetConnectionString("DefaultConnection")
         ?? throw new InvalidOperationException("Missing conn-string");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(cs));

// 2. Identity
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(opt =>
{
    opt.SignIn.RequireConfirmedAccount = false;
    opt.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// 3. Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", p => p.RequireRole("Admin"))
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole("Admin")
        .Build());

// 4. Razor Pages
builder.Services.AddRazorPages(opt =>
{
    opt.Conventions.AuthorizeFolder("/", "AdminOnly");
    opt.Conventions.AllowAnonymousToAreaFolder("Identity", "/Account");
});

// ------------------------------------------------------------------
// Adapter services so the stock Identity UI (which asks for
// <IdentityUser>) can reuse the instances registered for <ApplicationUser>
// ------------------------------------------------------------------


var app = builder.Build();

// 5. HTTP pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.Run();