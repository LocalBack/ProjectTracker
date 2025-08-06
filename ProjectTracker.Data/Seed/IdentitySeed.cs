using Microsoft.AspNetCore.Identity;
using ProjectTracker.Core.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ProjectTracker.Data.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {
            var roles = new[]
            {
                new { Name = "Admin", Description = "Full administrative access to the system" },
                new { Name = "Manager", Description = "Can manage projects and employees" },
                new { Name = "Employee", Description = "Basic user access with work logging capabilities" },
                new { Name = "ReadOnly", Description = "View-only access to the system" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = role.Name,
                        // Add Description property if your ApplicationRole has it
                        // Description = role.Description 
                    });
                }
            }
        }

        public static async Task SeedDefaultAdminAsync(UserManager<ApplicationUser> userManager)
        {
            const string adminEmail = "admin@projecttracker.com";
            const string adminPassword = "Admin@123!";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Seed roles first
            await SeedRolesAsync(roleManager);

            // Then seed default admin user
            await SeedDefaultAdminAsync(userManager);
        }
    }
}