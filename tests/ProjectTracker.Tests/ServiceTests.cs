using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Context;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Implementations;
using Xunit;

namespace ProjectTracker.Tests
{
    public class ServiceTests
    {
        private AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task UserService_ToggleActive_ShouldFlipFlag()
        {
            using var context = CreateContext();
            var user = new ApplicationUser { Id = 1, UserName = "u", IsActive = true };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var service = new UserService(context);
            await service.ToggleActiveAsync(user.Id.ToString());

            var updated = await context.Users.FindAsync(1);
            Assert.False(updated!.IsActive);
        }

        [Fact]
        public async Task MaintenanceService_AddSchedule_ShouldSetNextMaintenanceDate()
        {
            using var context = CreateContext();
            var equipment = new Equipment { Id = 1, Name = "Eq", SerialNumber = "S1", Type = "T", PurchaseDate = DateTime.Today };
            context.Equipments.Add(equipment);
            await context.SaveChangesAsync();

            var dto = new MaintenanceScheduleDto
            {
                MaintenanceType = "Test",
                IntervalDays = 30,
                LastMaintenanceDate = DateTime.Today
            };

            var service = new MaintenanceService(context);
            await service.AddScheduleAsync(equipment.Id, dto);

            var schedule = await context.MaintenanceSchedules.FirstAsync();
            Assert.Equal(DateTime.Today.AddDays(30), schedule.NextMaintenanceDate.Date);
        }
    }
}
