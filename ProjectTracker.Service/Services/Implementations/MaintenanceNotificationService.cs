
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectTracker.Service.Services.Interfaces;


namespace ProjectTracker.Service.Services.Implementations
{
    public class MaintenanceNotificationService : BackgroundService
    {

        private readonly IServiceProvider _services;
        private readonly ILogger<MaintenanceNotificationService> _logger;

        public MaintenanceNotificationService(IServiceProvider services, ILogger<MaintenanceNotificationService> logger)
        {
            _services = services;

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
                    .Distinct();
                using var scope = _services.CreateScope();
                var service = scope.ServiceProvider.GetRequiredService<IMaintenanceScheduleService>();
                var due = await service.GetDueAsync();
                foreach (var item in due)
                {
                    _logger.LogInformation("Maintenance task due for equipment {Equipment} on {Date}", item.EquipmentName, item.NextMaintenanceDate);
                    await service.MarkNotifiedAsync(item.Id);
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            }
        }
    }
}
