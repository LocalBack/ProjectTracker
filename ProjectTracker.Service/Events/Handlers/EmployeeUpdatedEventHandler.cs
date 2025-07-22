using MediatR;
using System.Threading;
using System.Threading.Tasks;
using ProjectTracker.Core.Events;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Service.Events.Handlers
{
    public class EmployeeUpdatedEventHandler : INotificationHandler<EmployeeUpdatedEvent>
    {
        private readonly IRepository<ProjectEmployee> _projectEmployeeRepository;

        public EmployeeUpdatedEventHandler(IRepository<ProjectEmployee> projectEmployeeRepository)
        {
            _projectEmployeeRepository = projectEmployeeRepository;
        }

        public async Task Handle(EmployeeUpdatedEvent notification, CancellationToken cancellationToken)
        {
            var employee = notification.Employee;

            var relatedProjects = await _projectEmployeeRepository.GetAsync(
                pe => pe.EmployeeId == employee.Id,
                includes: pe => pe.Employee
            );

            foreach (var projectEmployee in relatedProjects)
            {
                projectEmployee.Employee = employee; // Senkronizasyon işlemi
            }

            await _projectEmployeeRepository.UpdateRangeAsync(relatedProjects);
        }
    }
}
