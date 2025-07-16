using ProjectTracker.Service.DTOs;

namespace ProjectTracker.Service.Services.Interfaces
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> GetProjectByIdAsync(int id);
        Task<ProjectDto> CreateProjectAsync(ProjectDto projectDto);
        Task<ProjectDto> UpdateProjectAsync(int id, ProjectDto projectDto);
        Task<bool> DeleteProjectAsync(int id);
        Task<int> GetProjectCountAsync();
        Task<IQueryable<ProjectDto>> GetAllProjectsQueryableAsync();

        // Yeni metodlar ekleyin
        //Task<bool> GetActiveProjectsAsync();
    }
}