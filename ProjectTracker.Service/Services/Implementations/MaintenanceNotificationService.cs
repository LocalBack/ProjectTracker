using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectTracker.Data.Context;

namespace ProjectTracker.Service.Services.Implementations
{
    public class MaintenanceNotificationService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<MaintenanceNotificationService> _logger;

        public MaintenanceNotificationService(
            IServiceScopeFactory scopeFactory,
            IEmailSender emailSender,
            ILogger<MaintenanceNotificationService> logger)
        {
            _scopeFactory = scopeFactory;
            _emailSender = emailSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckSchedulesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while checking maintenance schedules");
                }

                // Run once per day
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task CheckSchedulesAsync(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var dueSchedules = await context.MaintenanceSchedules
                .Include(ms => ms.Equipment)
                .Where(ms => ms.NextMaintenanceDate <= DateTime.Today && !ms.IsNotificationSent)
                .ToListAsync(token);

            foreach (var schedule in dueSchedules)
            {
                var projectId = schedule.Equipment.ProjectId;

                var emails = await context.UserProjects
                    .Include(up => up.User)
                    .Where(up => up.ProjectId == projectId)
                    .Select(up => up.User.Email)
                    .Where(email => email != null)
                    .Distinct()
                    .ToListAsync(token);

                foreach (var email in emails)
                {
                    var subject = $"Maintenance Due: {schedule.Equipment.Name}";
                    var body = $"Maintenance for {schedule.Equipment.Name} ({schedule.MaintenanceType}) is due on {schedule.NextMaintenanceDate:d}.";
                    await _emailSender.SendEmailAsync(email!, subject, body);
                }

                schedule.IsNotificationSent = true;
            }

            if (dueSchedules.Count > 0)
            {
                await context.SaveChangesAsync(token);
            }
        }
    }
}
