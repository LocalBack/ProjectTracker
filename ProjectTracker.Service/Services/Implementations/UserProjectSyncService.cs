using ProjectTracker.Service.Services.Interfaces;

public class UserProjectSyncService : IUserProjectSyncService
{
    private readonly IEmployeeService _employeeService;
    private readonly IUserProjectService _userProjectService;

    public UserProjectSyncService(IEmployeeService employeeService, IUserProjectService userProjectService)
    {
        _employeeService = employeeService;
        _userProjectService = userProjectService;
    }

    public async Task EnsureUserProjectsSyncedAsync(int userId)
    {
        var employeeDto = await _employeeService.GetEmployeeByUserIdAsync(userId);
        if (employeeDto == null) return;

        foreach (var projectId in employeeDto.Projects.Select(p => p.Id))
        {
            bool alreadyExists = await _userProjectService.ExistsAsync(userId, projectId);
            if (!alreadyExists)
            {
                await _userProjectService.CreateUserProjectAsync(userId, projectId);
            }
        }
    }
}
