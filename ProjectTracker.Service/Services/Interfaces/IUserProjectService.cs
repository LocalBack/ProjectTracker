using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IUserProjectService
    {
        Task<bool> ExistsAsync(int userId, int projectId);
        Task CreateUserProjectAsync(int userId, int projectId);
        Task<IEnumerable<ProjectDto>> GetProjectsByUserIdAsync(int userId);
    }
}
