using Microsoft.AspNetCore.Identity;
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Data.Seed
{
    public static class IdentitySeed
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            // Rolleri oluştur
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "Admin",
                    Description = "Sistem yöneticisi"
                });
            }

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new ApplicationRole
                {
                    Name = "User",
                    Description = "Standart kullanıcı"
                });
            }

            // Admin kullanıcı oluştur
            var adminEmail = "admin@projecttracker.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Sistem",
                    LastName = "Yöneticisi",
                    EmailConfirmed = true,
                    CreatedDate = DateTime.Now
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}