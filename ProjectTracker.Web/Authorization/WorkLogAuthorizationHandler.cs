using Microsoft.AspNetCore.Authorization;
using ProjectTracker.Core.Entities;

namespace ProjectTracker.Web.Authorization
{
    public class WorkLogOwnerRequirement : IAuthorizationRequirement { }

    public class WorkLogAuthorizationHandler : AuthorizationHandler<WorkLogOwnerRequirement, WorkLog>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            WorkLogOwnerRequirement requirement,
            WorkLog resource)
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            // Admins can access all work logs
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Managers can access work logs for their projects
            if (context.User.IsInRole("Manager"))
            {
                // Add logic to check if manager manages this project
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            // Employees can only access their own work logs
           // if (resource.Employee?.UserId == int.Parse(userId))
         //   {
          //      context.Succeed(requirement);
          //  }

            return Task.CompletedTask;
        }
    }
}