using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Core.Entities;
using ProjectTracker.Service.Services.Interfaces;
using System.Linq;

namespace ProjectTracker.Web.Authorization
{
    public class WorkLogOwnerRequirement : IAuthorizationRequirement { }

    public class WorkLogAuthorizationHandler : AuthorizationHandler<WorkLogOwnerRequirement, WorkLog>
    {
        private readonly IEmployeeService _employeeService;

        public WorkLogAuthorizationHandler(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            WorkLogOwnerRequirement requirement,
            WorkLog resource)
        {
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return;
            }

            if (context.User.IsInRole("Admin") || context.User.IsInRole("Manager"))
            {
                context.Succeed(requirement);
                return;
            }

            var employee = await _employeeService.GetEmployeeByUserIdAsync(userId);
            if (employee == null)
            {
                return;
            }

            if (resource.EmployeeId == employee.Id)
            {
                context.Succeed(requirement);
            }
        }
    }
}