using Microsoft.Extensions.Hosting;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Admin
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
