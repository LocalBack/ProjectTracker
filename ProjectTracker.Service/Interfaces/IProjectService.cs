using ProjectTracker.Service.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> GetProjectByIdAsync(int id);
        Task<int> GetProjectCountAsync();
        Task<IQueryable<ProjectDto>> GetAllProjectsQueryableAsync();  // Fixed return type
        Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);
        Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto);
        Task<bool> DeleteProjectAsync(int id);
        Task<bool> ProjectExistsAsync(int id);
    }
}