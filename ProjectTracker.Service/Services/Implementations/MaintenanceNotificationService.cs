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
                await CheckSchedulesAsync();
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task CheckSchedulesAsync()
        {
            using var scope = _services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IMaintenanceScheduleService>();
            var due = await service.GetDueAsync();
            foreach (var item in due)
            {
                _logger.LogInformation("Maintenance task due for equipment {Equipment} on {Date}", item.EquipmentName, item.NextMaintenanceDate);
                await service.MarkNotifiedAsync(item.Id);
            }
        }
    }
}
