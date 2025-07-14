using AutoMapper;
using ProjectTracker.Core.Entities;
using ProjectTracker.Data.Repositories;
using ProjectTracker.Service.DTOs;
using ProjectTracker.Service.Services.Interfaces;

namespace ProjectTracker.Service.Services.Implementations
{
    public class ProjectService : IProjectService
    {
        private readonly IRepository<Project> _projectRepository;
        private readonly IMapper _mapper;

        public ProjectService(IRepository<Project> projectRepository, IMapper mapper)
        {
            _projectRepository = projectRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }

        public async Task<ProjectDto?> GetProjectByIdAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            return project == null ? null : _mapper.Map<ProjectDto>(project);
        }

        public async Task<ProjectDto> CreateProjectAsync(CreateProjectDto createProjectDto)
        {
            var project = _mapper.Map<Project>(createProjectDto);
            await _projectRepository.AddAsync(project);
            await _projectRepository.SaveAsync();

            return _mapper.Map<ProjectDto>(project);
        }

        public async Task UpdateProjectAsync(int id, ProjectDto projectDto)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new Exception($"Project with id {id} not found");

            _mapper.Map(projectDto, project);
            _projectRepository.Update(project);
            await _projectRepository.SaveAsync();
        }

        public async Task DeleteProjectAsync(int id)
        {
            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                throw new Exception($"Project with id {id} not found");

            project.IsActive = false; // Soft delete
            _projectRepository.Update(project);
            await _projectRepository.SaveAsync();
        }

        public async Task<IEnumerable<ProjectDto>> GetActiveProjectsAsync()
        {
            var projects = await _projectRepository.FindAsync(p => p.IsActive);
            return _mapper.Map<IEnumerable<ProjectDto>>(projects);
        }
    }
}