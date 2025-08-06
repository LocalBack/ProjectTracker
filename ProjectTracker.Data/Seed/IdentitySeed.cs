using Microsoft.AspNetCore.Identity;
using ProjectTracker.Core.Entities;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Data.Context;
using System.Linq;

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

        public static async Task SeedDefaultManagerAsync(UserManager<ApplicationUser> userManager)
        {
            const string managerEmail = "manager@projecttracker.com";
            const string managerPassword = "Manager@123!";

            if (await userManager.FindByEmailAsync(managerEmail) == null)
            {
                var managerUser = new ApplicationUser
                {
                    UserName = managerEmail,
                    Email = managerEmail,
                    FirstName = "Default",
                    LastName = "Manager",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(managerUser, managerPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(managerUser, "Manager");
                }
            }
        }

        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Seed roles first
            await SeedRolesAsync(roleManager);

            // Seed users
            await SeedDefaultAdminAsync(userManager);
            await SeedDefaultManagerAsync(userManager);

            // Seed sample project and equipment
            if (!context.Projects.Any())
            {
                var project = new Project
                {
                    Name = "Drone",
                    Description = "Sample project",
                    StartDate = DateTime.Today,
                    Budget = 1000
                };
                context.Projects.Add(project);
                await context.SaveChangesAsync();

                var servo = new Equipment
                {
                    Name = "Servo",
                    SerialNumber = "SERVO-001",
                    Type = "Motor",
                    PurchaseDate = DateTime.Today,
                    ProjectId = project.Id
                };
                var esc = new Equipment
                {
                    Name = "ESC",
                    SerialNumber = "ESC-001",
                    Type = "Controller",
                    PurchaseDate = DateTime.Today,
                    ProjectId = project.Id
                };
                context.Equipments.AddRange(servo, esc);
                await context.SaveChangesAsync();

                context.MaintenanceSchedules.Add(new MaintenanceSchedule
                {
                    EquipmentId = servo.Id,
                    MaintenanceType = "General",
                    IntervalDays = 180,
                    LastMaintenanceDate = DateTime.Today,
                    NextMaintenanceDate = DateTime.Today.AddDays(180)
                });
                await context.SaveChangesAsync();
            }
        }
    }
}