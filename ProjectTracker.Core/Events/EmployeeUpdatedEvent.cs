using MediatR;
using ProjectTracker.Core.Entities;


namespace ProjectTracker.Core.Events
{
    public class EmployeeUpdatedEvent : INotification
    {
        public Employee Employee { get; }

        public EmployeeUpdatedEvent(Employee employee)
        {
            Employee = employee;
        }
    }
}
