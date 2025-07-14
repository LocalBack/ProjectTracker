using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto?> GetProjectByIdAsync(int id);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto);
        Task UpdateProjectAsync(int id, ProjectDto projectDto);
        Task DeleteProjectAsync(int id);
        Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync();
    }
}